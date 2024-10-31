using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace LikeButtonAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApiResponse> CreateUserAsync(UserDto userDto);
        Task<User> LoginAsync(string email, string password);
        Task<User> GetUserByIdAsync(Guid userId);
        // Task<User> GetUserByEmailAsync(string email);

    }
}