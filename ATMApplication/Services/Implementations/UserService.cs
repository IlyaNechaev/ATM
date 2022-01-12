using ATMApplication.Extensions;
using ATMApplication.Models;
using ATMApplication.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace ATMApplication.Services
{
    public class UserService : IUserService
    {
        ISecurityService SecurityService { get; init; }
        IConfiguration Configuration { get; init; }
        IDbService DbService { get; init; }
        IRepositoryFactory RepositoryFactory { get; init; }
        ICookieService CookieService { get; init; }
        ILogger Logger { get; init; }

        public UserService(IRepositoryFactory repositoryFactory,
                           IDbService dbService,
                           ISecurityService securityService,
                           IConfiguration configuration,
                           ICookieService cookieService,
                           ILogger logger)
        {
            RepositoryFactory = repositoryFactory;
            DbService = dbService;
            SecurityService = securityService;
            Configuration = configuration;
            CookieService = cookieService;
            Logger = logger;
        }

        public async Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages)> RegisterUser(RegisterEditModel registerModel)
        {
            return await RegisterUser(registerModel, SecurityService);
        }

        public async Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages)> RegisterUser(RegisterEditModel registerModel,
                                                                                                           ISecurityService securityService)
        {
            (bool HasErrors, List<(string Key, string Message)> ErrorMessages) result = new();
            result.ErrorMessages = new List<(string Key, string Message)>();

            var UserRepository = RepositoryFactory.GetRepository<User>();

            // Существует ли пользователь с таким логином
            if (await UserRepository.GetAsync(user => user.Login == registerModel.Login) != null)
            {
                result.ErrorMessages.Add(("Login", "Логин уже используется"));
            }
            else if (await UserRepository.GetAsync(user => user.Email == registerModel.Email) != null)
            {
                result.ErrorMessages.Add(("Email", "Email уже используется"));
            }
            else
            {
                User user = registerModel.ToUser(securityService);

                await UserRepository.AddAsync(user);
            }
            result.HasErrors = result.ErrorMessages.Count > 0;

            return result;
        }

        public async Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages, string Token)> SignInUser(string userLogin,
                                                                                                         string userPassword)
        {
            return await SignInUser(userLogin, userPassword, SecurityService);
        }

        public async Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages, string Token)> SignInUser(string userLogin,
                                                                                                         string userPassword,
                                                                                                         ISecurityService securityService)
        {
            (bool HasErrors, List<(string Key, string Message)> ErrorMessages, string Token) result = new();
            result.ErrorMessages = new List<(string Key, string Message)>();

            var UserRepository = RepositoryFactory.GetRepository<User>();
            User validUser;

            try
            {
                validUser = (await UserRepository.GetAsync(user => user.Login == userLogin)).Single();
            }
            catch
            {
                throw;
            }

            var token = string.Empty;

            // Если в базе нет пользователя с такими логином и паролем
            if (validUser == null || !securityService.ValidatePassword(userPassword, validUser?.PasswordHash))
            {
                result.ErrorMessages.Add(("", "Неверно введен логин или пароль"));
            }
            else
            {
                token = GenerateJSONWebToken(validUser);
            }

            result.HasErrors = result.ErrorMessages.Count > 0;
            result.Token = token;

            return result;
        }

        public async Task LogoutUser(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            context.Response.Cookies.Delete(ClaimKey.CookiesId);
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var UserRepository = RepositoryFactory.GetRepository<User>();

            User user = null;

            try
            {
                user = (await UserRepository.GetAsync(u => u.Id.Equals(userId))).Single();
            }
            catch (RepositoryException ex)
            {
                Logger.LogError(ex.FullMessage);
            }

            return user;
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                var UserRepository = RepositoryFactory.GetRepository<User>();
                await UserRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsPasswordValid(Guid userId, string password)
        {
            var UserRepository = RepositoryFactory.GetRepository<User>();

            User user = null;

            try
            {
                user = await UserRepository.GetSingleAsync(user => user.Id == userId);
            }
            catch (RepositoryException ex)
            {
                Logger.LogError(ex.FullMessage);
            }

            return user.PasswordHash.Equals(SecurityService.GetPasswordHash(password));
        }

        public async Task ChangePassword(Guid userId, string newPassword)
        {
            var UserRepository = RepositoryFactory.GetRepository<User>();
            User user = null;

            try
            {
                user = await UserRepository.GetSingleAsync(user => user.Id == userId);
            }
            catch (RepositoryException ex)
            {
                Logger.LogError(ex.FullMessage);
                return;
            }

            user.PasswordHash = SecurityService.GetPasswordHash(newPassword);

            await UserRepository.UpdateAsync(user);
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimKey.Login, user.Login),
                new Claim(ClaimKey.FirstName, user.FirstName),
                new Claim(ClaimKey.MiddleName, user.MiddleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(Configuration["Jwt:Issuer"],
              Configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
