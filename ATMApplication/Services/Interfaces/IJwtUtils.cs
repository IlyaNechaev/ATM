using ATMApplication.Models;
using System;

namespace ATMApplication.Services
{
    public interface IJwtUtils
    {
        public string GenerateJSONWebToken(User user);

        public Guid? ValidateToken(string token);

    }
}
