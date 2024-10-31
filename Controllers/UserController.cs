using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LikeButtonAPI.DTOs.Requests;
using LikeButtonAPI.Repositories;
using LikeButtonAPI.Services.Implementation;
using LikeButtonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LikeButtonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
       private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var result = await _userService.CreateUser(userDto);

        return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto  loginDto)
        {
            var result = await _userService.Login(loginDto.Email, loginDto.Password);
            return Ok(result);
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}