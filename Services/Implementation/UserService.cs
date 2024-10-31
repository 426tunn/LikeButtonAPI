using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Repositories;
using LikeButtonAPI.Repositories.Interfaces;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LikeButtonAPI.Services.Implementation
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

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),  // Token expiration time
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
        public async Task<ApiResponse> CreateUser(UserDto userDto)
        {
            try
            {
                var response = await _userRepository.CreateUserAsync(userDto);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create user: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetUserById(Guid id)
        {
            try
            {

                var user = await _userRepository.GetUserByIdAsync(id);
                return new ApiResponse("User retrieved successfully.", 200, user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user: {ex.Message}");
            }
        }

        public async Task<ApiResponse> Login(string email, string password)
        {
            try
            {
                var user = await _userRepository.LoginAsync(email, password);
                if (user == null){
                    throw new Exception($"User not found.");
                }

                // Generate JWT Token
            var token = GenerateJwtToken(user);
            
                return new ApiResponse("Login successful.", 200, new { token });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to log in user: {ex.Message}");
            }
        }
    }
}
