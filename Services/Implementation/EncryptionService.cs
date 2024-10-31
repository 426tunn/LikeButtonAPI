using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LikeButtonAPI.Services.Implementation
{
    public class EncryptionService
    {
         public string EncryptPassword(string password)
        {
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return HashedPassword;
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
          {
             // Verify the entered password against the stored hash
             return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
          }
    }
}