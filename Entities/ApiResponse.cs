using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.Entities
{
    public class ApiResponse
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public object? Data { get; set; }

        public ApiResponse(string message, int statusCode, object data) 
        {
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }

    }
}