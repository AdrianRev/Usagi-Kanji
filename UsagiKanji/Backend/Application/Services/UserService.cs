using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Result<User>> SignUpAsync(string username, string email, string password)
        {
            var userByUsername = !string.IsNullOrWhiteSpace(username)
                ? await _userRepository.GetByUsernameOrEmailAsync(username)
                : null;
            if (userByUsername != null)
            {
                return Result.Fail<User>(
                    new Error("Account using this username already exists")
                        .WithMetadata("StatusCode", 409)
                        .WithMetadata("field", "username")
                );
            }

            var userByEmail = userByUsername == null && !string.IsNullOrWhiteSpace(email)
                ? await _userRepository.GetByUsernameOrEmailAsync(email)
                : null;

            if (userByEmail != null)
            {
                return Result.Fail<User>(
                    new Error("Account using this email already exists")
                        .WithMetadata("StatusCode", 409)
                        .WithMetadata("field", "email")
                );
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash
            };

            await _userRepository.AddAsync(user);
            return Result.Ok(user);
        }

        public async Task<Result<string>> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _userRepository.GetByUsernameOrEmailAsync(usernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return Result.Fail<string>(
                    new Error("Invalid username/email or password")
                        .WithMetadata("StatusCode", 401)
                );
            }

            var token = GenerateJwtToken(user);
            return Result.Ok(token);
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

