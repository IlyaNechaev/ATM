using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public interface IBankService
    {
        public Task TransferMoney(BankAccount source, BankAccount dest, decimal sum);

        public Task<IEnumerable<BankAccount>> GetUserBankAccounts(string userId);

        public Task<BankAccount> GetBankAccountById(string accountId);

        public Task<BankAccount> CreateBankAccountForUser(User user, BankAccountType accountType, decimal moneyLimit = 0);

        public Task AddNewCard(BankAccount account, Card card);
    }
}
