using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Repository.Implementation
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IConfiguration _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

        public APIResult<string> AuthenticateCustomer(LoginDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user == null) return new APIErrorResult<string>("Account doesn't exist.");

            // Validate Role
            if (!user.Role.Equals("CUSTOMER")) return new APIErrorResult<string>("Please use Store Login Page to access Store account");

            // Validate user by status
            if (user.Status == 0) return new APIErrorResult<string>("This account has been blocked. Contact administrator to unblock.");

            // Check for password encrypt match
            if (!BC.Verify(request.Password, user.Password)) return new APIErrorResult<string>("Wrong password.");

            // Create user identity with ID, Email, Name, Role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT Token with expiration time = 3 hours
            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new APISuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public APIResult<string> AuthenticateStore(LoginDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user == null) return new APIErrorResult<string>("Account doesn't exist.");

            // Validate Role
            if (!user.Role.Equals("STORE")) return new APIErrorResult<string>("Please use Website Login Page to access your account");

            // Validate user by status
            if (user.Status == 0) return new APIErrorResult<string>("This account has been blocked. Contact administrator to unblock.");

            // Check for password encrypt match
            if (!BC.Verify(request.Password, user.Password)) return new APIErrorResult<string>("Wrong password.");

            if(user.Store == null) return new APIErrorResult<string>("Invalid store account");

            // Create user identity with ID, Email, Name, Role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("StoreName", user.Store.Name),
                new Claim("StoreLogo", user.Store.LogoImage)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT Token with expiration time = 3 hours
            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new APISuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public APIResult<bool> RegisterCustomer(RegisterCustomerDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user != null) return new APIErrorResult<bool>("This Email has already been used.");

            // Validate Confirm Password
            if (!request.Password.Equals(request.ConfirmPassword)) return new APIErrorResult<bool>("Confirm Password doesn't match.");

            UserAccount newUser = new()
            {
                UserId = 0,
                Email = request.Email,
                Name = request.Name,
                Password = BC.HashPassword(request.Password),
                Phone = request.Phone,
                Role = "CUSTOMER",
                EmailVerified = 1,
                Status = 1,
                StoreId = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            UserAccountDAO.CreateUser(newUser);
            if (newUser.UserId != 0)
            {
                return new APISuccessResult<bool>();
            }
            return new APIErrorResult<bool>("Can't add new user to database.");
        }

        public APIResult<bool> RegisterStore(RegisterStoreDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user != null) return new APIErrorResult<bool>("This Email has already been used.");

            // Validate Confirm Password
            if (!request.Password.Equals(request.ConfirmPassword)) return new APIErrorResult<bool>("Confirm Password doesn't match.");

            // Add new Store
            // Validate store name and address
            if (string.IsNullOrEmpty(request.NewStoreName)) return new APIErrorResult<bool>("Store name required!");
            if (string.IsNullOrEmpty(request.NewStoreAddress)) return new APIErrorResult<bool>("Store address required!");
            Store newStore = new()
            {
                StoreId = 0,
                Name = request.NewStoreName,
                Address = request.NewStoreAddress,
                Status = 1,
                Description = request.NewStoreDescription,
                LogoImage = request.NewStoreLogoImage,
                CoverImage = request.NewStoreCoverImage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            StoreDAO.CreateStore(newStore);
            if (newStore.StoreId != 0)
            {
                UserAccount newUser = new()
                {
                    UserId = 0,
                    Email = request.Email,
                    Name = request.Name,
                    Password = BC.HashPassword(request.Password),
                    Phone = request.Phone,
                    Role = "STORE",
                    Status = 1,
                    EmailVerified = 1,
                    StoreId = newStore.StoreId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                UserAccountDAO.CreateUser(newUser);

                if (newUser.UserId == 0)
                {
                    return new APIErrorResult<bool>("Can't add new user to database.");
                }
                newStore.UserId = newUser.UserId;
                StoreDAO.UpdateStore(newStore);
                return new APISuccessResult<bool>();
            }

            return new APIErrorResult<bool>("Can't add new store to database.");
        }
        // Admin get all users filter by role
        public ClientUserViewListDTO GetAllUsers(int page, string? roleSearch)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            List<UserDetailViewDTO?> userByAdmin = UserAccountDAO
                .GetUsersByAdmin(page, size, roleSearch)
                .Select(x => Mapper.ToUserDetailViewDTO(x))
                .ToList();

            // Get count of userrs by search/filter
            int userCount = UserAccountDAO.CountUsersByAdmin(roleSearch);
            int totalPages = (int)Math.Ceiling((double)userCount / size);
            List<int> pageNumbers = new List<int>();
            if (totalPages > 0)
            {
                int start = Math.Max(1, page - 2);
                int end = Math.Min(page + 2, totalPages);

                if (totalPages > 5)
                {
                    if (end == totalPages) start = end - 4;
                    else if (start == 1) end = start + 4;
                }
                else
                {
                    start = 1;
                    end = totalPages;
                }
                pageNumbers = Enumerable.Range(start, end - start + 1).ToList();
            }

            return new ClientUserViewListDTO()
            {
                UsersPaginated = userByAdmin,
                Page = page,
                Size = size,
                Role = roleSearch,
                PageNumbers = pageNumbers,
                TotalCount = userCount,
                TotalPage = totalPages
            };
        }
        //Admin get user detail
        public APIResult<UserDetailViewDTO> GetUserDetail(long userId)
        {
            UserAccount user = UserAccountDAO.FindUserById(userId);

            if (user == null) return new APIErrorResult<UserDetailViewDTO>("Can't find user.");

            if (user.Store == null)
            {
                UserDetailViewDTO newUser = new()
                {
                    Id = userId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Status = user.Status,
                };
                return new APISuccessResult<UserDetailViewDTO>(newUser);
            }
            else
            {
                UserDetailViewDTO newUser = new()
                {
                    Id = userId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Status = user.Status,
                    StoreName = user.Store.Name,
                    LogoImage = user.Store.LogoImage,
                    CoverImage = user.Store.CoverImage,
                    Description = user.Store.Description,
                };
                return new APISuccessResult<UserDetailViewDTO>(newUser);
            }
        }
        //Admin deactivate user account
        public APIResult<string> DeactivateAccount(long userId)
        {
            UserAccount? userEntity = UserAccountDAO.FindUserById(userId);
            if (userEntity == null) return new APIErrorResult<string>("Cannot find this user!");

            // Change order status to 5: waiting for cancel approval
            if (userEntity.Status == 1)
            {
                userEntity.Status = 0;
                userEntity.UpdatedAt = DateTime.Now;
                UserAccountDAO.UpdateUser(userEntity);
            }
            else
            {
                return new APIErrorResult<string>("This account is already deactivated!");
            }

            return new APISuccessResult<string>("Deactivate account successfully!");
        }
        //Admin activate user account
        public APIResult<string> ActivateAccount(long userId)
        {
            UserAccount? userEntity = UserAccountDAO.FindUserById(userId);
            if (userEntity == null) return new APIErrorResult<string>("Cannot find this user!");

            // Change order status to 5: waiting for cancel approval
            if (userEntity.Status == 0)
            {
                userEntity.Status = 1;
                userEntity.UpdatedAt = DateTime.Now;
                UserAccountDAO.UpdateUser(userEntity);
            }
            else
            {
                return new APIErrorResult<string>("This account is already activated!");
            }

            return new APISuccessResult<string>("Account is now active!");
        }
        public APIResult<UserProfileViewDTO> GetCurrentCustomer(long currentUserId)
        {
            if (currentUserId == null) return new APIErrorResult<UserProfileViewDTO>("Can't find user.");

            UserAccount user = UserAccountDAO.FindUserById(currentUserId);

            if (user == null) return new APIErrorResult<UserProfileViewDTO>("This user is not existed.");

            UserProfileViewDTO newUser = new()
            {
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone
            };

            return new APISuccessResult<UserProfileViewDTO>(newUser);
        }

        public APIResult<bool> UpdateProfile(long currentUserId, UserProfileUpdateDTO profile)
        {
            if (profile == null) return new APIErrorResult<bool>("The new profile cannot be empty.");

            UserAccount user = UserAccountDAO.FindUserById(currentUserId);

            if (user == null) return new APIErrorResult<bool>("This user is not existed.");
            else
            {
                // Change user's name and phone
                user.Name = profile.Name;
                user.Phone = profile.Phone;

                UserAccountDAO.UpdateUser(user);
                return new APISuccessResult<bool>();
            }
        }

        public APIResult<bool> ChangePassword(long currentUserId, UserPasswordUpdateDTO password)
        {
            if (password == null) return new APIErrorResult<bool>("The new password cannot be empty.");

            UserAccount user = UserAccountDAO.FindUserById(currentUserId);

            if (!BC.Verify(password.OldPassword, user.Password))
            {
                return new APIErrorResult<bool>("The old password is not corrected.");
            }
            else if (password.NewPassword == password.OldPassword)
            {
                return new APIErrorResult<bool>("The new password must be diffent from old password.");
            }
            else if (password.ConfirmPassword != password.NewPassword)
            {
                return new APIErrorResult<bool>("The new password and confirm password are not the same.");
            }

            if (user == null) return new APIErrorResult<bool>("This user is not existed.");
            else
            {
                // Change user's password
                user.Password = BC.HashPassword(password.NewPassword);

                UserAccountDAO.UpdateUser(user);
                return new APISuccessResult<bool>();
            }
        }

        public APIResult<string> AuthenticateAdmin(LoginDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user == null) return new APIErrorResult<string>("Account doesn't exist.");

            // Validate Role
            if (!user.Role.Equals("ADMIN")) return new APIErrorResult<string>("Not admin account");

            // Validate user by status
            if (user.Status == 0) return new APIErrorResult<string>("This account has been blocked. Contact administrator to unblock.");

            // Check for password encrypt match
            if (!BC.Verify(request.Password, user.Password)) return new APIErrorResult<string>("Wrong password.");

            // Create user identity with ID, Email, Name, Role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT Token with expiration time = 3 hours
            var token = new JwtSecurityToken(
                _config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new APISuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
