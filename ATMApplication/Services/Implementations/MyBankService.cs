using ATMApplication.Models;
using ATMApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public class MyBankService : IBankService
    {
        IRepositoryFactory RepositoryFactory { get; init; }
        public MyBankService(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }
        public async Task<User> CreateUser(User user)
        {
            // Проверка необходимых параметров на null
            //#region NULL_CHECKING
            //var nullParams = new[] { firstName, middleName, phoneNumber }.Where(x => x is null);

            //if (nullParams.Count() > 0)
            //    throw new ArgumentNullException($"{string.Join(", ", nullParams.Select(x => nameof(x)))}");
            //#endregion

            var UserRepository = RepositoryFactory.GetRepository<User>();

            var isPhoneNumberExisted = (await UserRepository.GetAsync(u => u.PhoneNumber.Equals(phoneNumber))).Count() > 0;
            if (isPhoneNumberExisted)
                throw new ArgumentException($"Телефонный номер {phoneNumber} уже существует");

            do
            {
                user.Id = Guid.NewGuid();
            }
            while ((await UserRepository.GetAsync(u => u.Id.Equals(user.Id))).Count() > 0);

            await UserRepository.AddAsync(user);

            return user;
        }
    }
}
