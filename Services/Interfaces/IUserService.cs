using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Entities;

namespace LikeButtonAPI.Services.Interfaces
{
    
    public interface IUserService
    {
        Task<ApiResponse> Login( string email, string password);
        Task<ApiResponse> CreateUser(UserDto userDto);
        Task<ApiResponse> GetUserById(Guid id);
    }
}