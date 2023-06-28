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

        public APIResult<bool> Register(RegisterDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user != null) return new APIErrorResult<bool>("This Email has already been used.");

            // Validate Confirm Password
            if (!request.Password.Equals(request.ConfirmPassword)) return new APIErrorResult<bool>("Confirm Password doesn't match.");

            // Validate Role
            if (!(request.Role.Equals("CUSTOMER") || request.Role.Equals("STORE"))) return new APIErrorResult<bool>("Invalid Role");

            // If user is customer / store with existing store
            if (request.CreateNewStore == 0)
            {
                UserAccount newUser = new()
                {
                    UserId = 0,
                    Email = request.Email,
                    Name = request.Name,
                    Password = BC.HashPassword(request.Password),
                    Phone = request.Phone,
                    Role = request.Role,
                    EmailVerified = 1,
                    Status = 1,
                    StoreId = request.StoreId == 0 ? null : request.StoreId,
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

            // If user is store with new store
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
                    Role = request.Role,
                    Status = 1,
                    StoreId = newStore.StoreId,
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

            return new APIErrorResult<bool>("Can't add new store to database.");
        }
    }
}
