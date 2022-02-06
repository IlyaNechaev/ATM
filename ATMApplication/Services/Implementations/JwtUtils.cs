using ATMApplication.Extensions;
using ATMApplication.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ATMApplication.Services
{
    public class JwtUtils : IJwtUtils
    {
        IConfiguration Configuration { get; init; }

        public JwtUtils(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GenerateJSONWebToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[] {
                new Claim(ClaimKey.Login, user.Login),
                new Claim(ClaimKey.FirstName, user.FirstName),
                new Claim(ClaimKey.MiddleName, user.MiddleName),
                new Claim(ClaimKey.Id, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public Guid? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == ClaimKey.Id).Value);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
