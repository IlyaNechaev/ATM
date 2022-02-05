using ATMApplication.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Mapping
{
    public partial class MappingProfile
    {
        [PerformMapping]
        private void RegisterUserProfile()
        {
            CreateMap<RegisterEditModel, User>()
                .ForMember(user => user.FirstName, options => options.MapFrom(model => model.FirstName)) // Имя
                .ForMember(user => user.MiddleName, options => options.MapFrom(model => model.MiddleName)) // Фамилия
                .ForMember(user => user.LastName, options => options.MapFrom(model => model.LastName)) // Отчество
                .ForMember(user => user.Login, options => options.MapFrom(model => model.Login)) // Логин
                .ForMember(user => user.PasswordHash, options => options.MapFrom(model => _securityService.GetPasswordHash(model.Password))) // Пароль
                .ForMember(user => user.Email, options => options.MapFrom(model => model.Email)) // Email
                .ForMember(user => user.DateOfBirth, options => options.MapFrom(model => ConvertToDateTime(model.DateOfBirth))) // Дата рождения
                .ForMember(user => user.Gender, options => options.MapFrom(model => model.Gender)) // Пол
                .ForMember(user => user.PhoneNumber, options => options.MapFrom(model => model.PhoneNumber)); // Номер телефона
        }

        private DateTime ConvertToDateTime(string date)
        {
            return new DateTime(
                int.Parse(date.Split('.')[2]),
                int.Parse(date.Split('.')[1]),
                int.Parse(date.Split('.')[0])
                );
        }

        private Dictionary<string, Gender> GetGenderDict()
        {
            var genderDict = new Dictionary<string, Gender>();
            foreach (var gender in Enum.GetValues(typeof(Gender)))
            {
                genderDict.Add(gender.ToString(), (Gender)gender);
            }
            return genderDict;
        }
    }
}
