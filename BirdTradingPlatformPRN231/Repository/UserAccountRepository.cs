using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObjects.Models;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IConfiguration _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

        public APIResult<String> Authenticate(LoginDTO request)
        {
            // Get Account by email
            UserAccount? user = UserAccountDAO.FindUserByEmail(request.Email);

            // Validate user exist
            if (user == null) return new APIErrorResult<string>("Account doesn't exist.");

            // Validate user by status
            if(user.Status == 0) return new APIErrorResult<string>("This account has been blocked. Contact administrator to unblock.");

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
