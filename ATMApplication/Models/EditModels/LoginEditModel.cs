using ATMApplication.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATMApplication.Models
{
    public class LoginEditModel : IUserValidationModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        public string Password { get; set; }

        public string GetLogin() => Login;

        public string GetPassword() => Password;
    }
}
