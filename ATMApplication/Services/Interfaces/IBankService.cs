using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public interface IBankService
    {
        public Task<Transaction> TransferMoney(BankAccount source, BankAccount dest, decimal sum);
        public Task<Transaction> TransferMoney(BankAccount account, decimal sum, bool deposit = true);

        public Task<IEnumerable<BankAccount>> GetUserBankAccounts(string userId);

        public Task<BankAccount> GetBankAccountById(string accountId);

        public Task<BankAccount> CreateBankAccountForUser(User user, BankAccountType accountType, decimal moneyLimit = 0);

    }
}
