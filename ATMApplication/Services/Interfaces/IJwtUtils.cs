using ATMApplication.Models;
using System;
using System.Security.Claims;

namespace ATMApplication.Services
{
    public interface IJwtUtils
    {
        public string GenerateJSONWebToken(User user);

        public ClaimsPrincipal ValidateToken(string token);

    }
}
