using ATMApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public interface IUserService
    {
        // Регистрация нового пользователя
        public Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages)> RegisterUser(RegisterEditModel registerModel);
        public Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages)> RegisterUser(RegisterEditModel registerModel, ISecurityService securityService);

        // Вход в систему пользователя по логину и паролю
        public Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages, string Token)> SignInUser(string userLogin, string userPassword);
        public Task<(bool HasErrors, List<(string Key, string Message)> ErrorMessages, string Token)> SignInUser(string userLogin, string userPassword, ISecurityService securityService);

        public Task LogoutUser(HttpContext context);

        public Task<User> GetUserById(Guid userId);

        public Task UpdateUser(User user);

        public Task<bool> IsPasswordValid(Guid userId, string password);

        public Task ChangePassword(Guid userId, string newPassword);
    }
}
