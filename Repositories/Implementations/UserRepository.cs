using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.Data;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;
using LikeButtonAPI.Repositories.Interfaces;
using LikeButtonAPI.Services.Implementation;
using Microsoft.EntityFrameworkCore;

namespace LikeButtonAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly EncryptionService _encryptionService;

        public UserRepository(AppDbContext context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse> CreateUserAsync(UserDto userDto)
        {
            string hashedPassword = _encryptionService.EncryptPassword(userDto.Password);
            var newUser = new User {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = hashedPassword
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new ApiResponse("User created successfully.", 200, newUser);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) 
            {
                throw new Exception("User not found.");
            }

            return user;
        }

  public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

       bool isValidPassword = _encryptionService.VerifyPassword(password, user.Password);
       if (!isValidPassword){
        throw new Exception("Invalid password.");
       }

        return user;
    }

    
    }
}