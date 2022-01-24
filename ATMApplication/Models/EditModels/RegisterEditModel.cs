using ATMApplication.Validation;
using ATMApplication.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ATMApplication.Models
{
    public class RegisterEditModel : IUserValidationModel
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Не указано имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [Display(Name = "Фамилия")]
        public string MiddleName { get; set; }

        [Display(Name = "Отчество")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Не указан email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль повторно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required(ErrorMessage = "Введите номер телефона")]
        public string PhoneNumber { get; set; }

        [MinLength(10, ErrorMessage = "Не выбрана дата")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Не выбран пол")]
        public Gender Gender { get; set; }

        #region IUserValidationModel
        public string GetLogin() => Login;

        public string GetPassword() => Password;

        public string GetPhoneNumber() => PhoneNumber;

        public string GetEmail() => Email;
        #endregion

        //public User ToUser(ISecurityService securityService)
        //{
        //    var user = new User();

        //    user.FirstName = FirstName;
        //    user.MiddleName = MiddleName;
        //    user.LastName = LastName;
        //    user.Login = Login;
        //    user.PasswordHash = securityService.GetPasswordHash(Password);
        //    user.Email = Email;
        //    user.DateOfBirth = new DateTime(
        //        int.Parse(DateOfBirth.Split('.')[2]),
        //        int.Parse(DateOfBirth.Split('.')[1]),
        //        int.Parse(DateOfBirth.Split('.')[0])
        //        );
        //    user.Gender = GetGenderDict().GetValueOrDefault(Gender);
        //    user.PhoneNumber = PhoneNumber;

        //    return user;

        //    // ЛОКАЛЬНАЯ ФУНКЦИЯ
        //    // Генерирует словарь (название пола; соответствующее значение в Models.Gender)
        //    Dictionary<string, Gender> GetGenderDict()
        //    {
        //        var genderDict = new Dictionary<string, Gender>();
        //        foreach (var gender in Enum.GetValues(typeof(Gender)))
        //        {
        //            genderDict.Add(gender.ToString(), (Gender)gender);
        //        }
        //        return genderDict;
        //    }
        //}
    }
}
