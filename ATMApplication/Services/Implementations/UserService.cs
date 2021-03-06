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
using ATMApplication.Validation;
using AutoMapper;

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
        IMapper Mapper { get; init; }
        IJwtUtils JwtUtils { get; init; }

        public UserService(IRepositoryFactory repositoryFactory,
                           IDbService dbService,
                           ISecurityService securityService,
                           IConfiguration configuration,
                           ICookieService cookieService,
                           IMapper mapper,
                           ILogger<UserService> logger,
                           IJwtUtils jwtUtils)
        {
            RepositoryFactory = repositoryFactory;
            DbService = dbService;
            SecurityService = securityService;
            Configuration = configuration;
            CookieService = cookieService;
            Logger  = logger;
            Mapper  = mapper;
            JwtUtils = jwtUtils;
        }

        public async Task<ValidationResult> RegisterUser(RegisterEditModel registerModel)
        {
            return await RegisterUser(registerModel, SecurityService);
        }

        public async Task<ValidationResult> RegisterUser(
            RegisterEditModel registerModel,
            ISecurityService securityService
            )
        {
            var validationResult = await ValidateRegisterModel(registerModel);

            if (validationResult.HasErrors)
            {
                return validationResult;
            }

            var user = Mapper.Map<User>(registerModel);

            var UserRepository = RepositoryFactory.GetRepository<User>();
            await UserRepository.AddAsync(user);

            return validationResult;
        }

        public async Task<(ValidationResult ValidationResult, User User)> GetLogInUser(LoginEditModel loginModel)
        {
            return await GetLogInUser(loginModel, SecurityService);
        }

        public async Task<(ValidationResult ValidationResult, User User)> GetLogInUser(
            LoginEditModel loginModel,
            ISecurityService securityService
            )
        {
            (var validationResult, var validUser) = await ValidateLoginModel(loginModel, securityService);

            if (validationResult.HasErrors)
            {
                return (validationResult, null);
            }

            return (validationResult, validUser);
        }

        public async Task<(ValidationResult ValidationResult, User User)> GetLogInUser(string login, string password)
        {
            return await GetLogInUser(login, password, SecurityService);
        }

        public async Task<(ValidationResult ValidationResult, User User)> GetLogInUser(string login, string password, ISecurityService securityService)
        {
            var loginModel = new LoginEditModel
            {
                Login = login,
                Password = password
            };

            return await GetLogInUser(loginModel, securityService);
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
                Logger?.LogError(ex.FullMessage);
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
                Logger?.LogError($"Exception: {ex.Message}");
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
                Logger?.LogError(ex.FullMessage);
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
                Logger?.LogError(ex.FullMessage);
                return;
            }

            user.PasswordHash = SecurityService.GetPasswordHash(newPassword);

            await UserRepository.UpdateAsync(user);
        }

        private async Task<ValidationResult> ValidateRegisterModel(IUserValidationModel model)
        {
            var result = new ValidationResult();

            var UserRepository = RepositoryFactory.GetRepository<User>();

            // Существует ли пользователь с таким логином
            if ((await UserRepository.GetAsync(user => user.Login == model.GetLogin())).Count() > 0)
            {
                result.AddMessage(nameof(User.Login), "Логин уже используется");
            }

            // Существует ли пользователь с таким Email
            if ((await UserRepository.GetAsync(user => user.Email == model.GetEmail())).Count() > 0)
            {
                result.AddMessage(nameof(User.Email), "Email уже используется");
            }

            // Существует ли пользователь с таким телефонным номером
            if ((await UserRepository.GetAsync(user => user.PhoneNumber == model.GetPhoneNumber())).Count() > 0)
            {
                result.AddMessage(nameof(User.PhoneNumber), "Телефонный номер уже используется");
            }

            return result;
        }

        private async Task<(ValidationResult, User validUser)> ValidateLoginModel(IUserValidationModel model, ISecurityService securityService)
        {
            var result = new ValidationResult();
            var UserRepository = RepositoryFactory.GetRepository<User>();
            User validUser = null;

            try
            {
                validUser = (await UserRepository.GetAsync(user => user.Login == model.GetLogin())).Single();
            }
            catch
            { }

            // Если в базе нет пользователя с такими логином и паролем
            if (validUser == null || !securityService.ValidatePassword(model.GetPassword(), validUser?.PasswordHash))
            {
                result.AddCommonMessage("Неверно введен логин или пароль");
            }

            return (result, validUser);
        }

        public async Task<ClaimsPrincipal> CreateUserPrincipal(User user)
        {
            if (user is null)
                return null;

            var claims = new Claim[]
            {
            new Claim(ClaimKey.Login, user.Login),
            new Claim(ClaimKey.FirstName, user.FirstName),
            new Claim(ClaimKey.Id, user.Id.ToString())
            };

            // Создаем объект ClaimsIdentity
            var claimId = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return await Task.FromResult(new ClaimsPrincipal(claimId));
        }
    }
}
