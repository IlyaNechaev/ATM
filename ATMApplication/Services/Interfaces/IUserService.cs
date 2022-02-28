using ATMApplication.Models;
using ATMApplication.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public interface IUserService
    {
        // Регистрация нового пользователя
        #region REGISTRATION
        public Task<ValidationResult> RegisterUser(RegisterEditModel registerModel);
        public Task<ValidationResult> RegisterUser(RegisterEditModel registerModel, ISecurityService securityService);
        #endregion

        #region GET_LOGIN_USER
        public Task<(ValidationResult ValidationResult, User User)> GetLogInUser(LoginEditModel loginModel);
        public Task<(ValidationResult ValidationResult, User User)> GetLogInUser(LoginEditModel loginModel, ISecurityService securityService);
        public Task<(ValidationResult ValidationResult, User User)> GetLogInUser(string login, string password);
        public Task<(ValidationResult ValidationResult, User User)> GetLogInUser(string login, string password, ISecurityService securityService);
        #endregion

        public Task<ClaimsPrincipal> CreateUserPrincipal(User user);

        public Task LogoutUser(HttpContext context);

        public Task<User> GetUserById(Guid userId);

        public Task UpdateUser(User user);

        public Task<bool> IsPasswordValid(Guid userId, string password);

        public Task ChangePassword(Guid userId, string newPassword);
    }
}
