using ATMApplication.Models;
using ATMApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using ATMApplication.Validation;

namespace ATMApplication.Services
{
    public class MyBankService : IBankService
    {
        IRepositoryFactory RepositoryFactory { get; init; }
        public MyBankService(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        public async Task<Transaction> TransferMoney(BankAccount source, BankAccount dest, decimal sum)
        {
            var validationResult = ValidateBankAccountForWithdraw(source, sum);
            if (validationResult.HasErrors)
            {
                throw new TransactionException(validationResult.CommonMessages[0]);
            }

            var TransactionRepository = RepositoryFactory.GetRepository<Transaction>();
            var transaction = new Transaction
            {
                AccountSender = source,
                AccountReceiver = dest,
                Sum = sum,
                TransactionTime = DateTime.Now
            };

            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();
            source.Balance -= sum;
            dest.Balance += sum;

            await BankAccountRepository.UpdateAsync(source);
            await BankAccountRepository.UpdateAsync(dest);
            await TransactionRepository.AddAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> TransferMoney(BankAccount account, decimal sum, bool deposit = true)
        {
            if (!deposit)
            {
                var validationResult = ValidateBankAccountForWithdraw(account, sum);

                if (validationResult.HasErrors)
                {
                    throw new TransactionException(validationResult.CommonMessages[0]);
                }
            }

            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();
            account.Balance = deposit ? account.Balance + sum : account.Balance - sum;
            await BankAccountRepository.UpdateAsync(account);

            return new Transaction
            {
                AccountReceiver = account,
                TransactionTime = DateTime.Now
            };
        }

        public async Task<IEnumerable<BankAccount>> GetUserBankAccounts(string userId)
        {
            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();

            return await BankAccountRepository.GetAsync(account => account.OwnerId == Guid.Parse(userId),
                nameof(BankAccount.Cards));
        }

        public async Task<BankAccount> CreateBankAccountForUser(User user, BankAccountType accountType, decimal moneyLimit = 0)
        {
            var accountNumberTask = CreateBankAccountNumber();
            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();

            var account = new BankAccount
            {
                BankAccountType = accountType,
                Cards = new(),
                Owner = user,
                Limit = accountType == BankAccountType.CREDIT ? moneyLimit : null,
                Balance = 0.0m,
                AccountNumber = await accountNumberTask
            };

            await BankAccountRepository.AddAsync(account);

            return account;
        }

        public async Task<BankAccount> GetBankAccountById(string accountId)
        {
            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();

            return await BankAccountRepository.GetSingleAsync(account => account.Id == Guid.Parse(accountId), 
                nameof(BankAccount.Cards), 
                nameof(BankAccount.Owner));
        }


        private async Task<string> CreateBankAccountNumber()
        {
            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();
            var random = new Random();
            var accountNumber = new StringBuilder();
            var isNumberExisted = true;

            do
            {
                for (int i = 0; i < 20; i++)
                {
                    accountNumber.Append(random.Next());
                }
                isNumberExisted = (await BankAccountRepository.GetAsync(account => account.AccountNumber.Equals(accountNumber.ToString()))).Count() > 0;
            }
            while(isNumberExisted);

            return accountNumber.ToString();
        }

        // Можно ли со счета снять/перевести деньги
        private ValidationResult ValidateBankAccountForWithdraw(BankAccount account, decimal sum)
        {
            var result = new ValidationResult();
            switch (account.BankAccountType)
            {
                case BankAccountType.CREDIT when account.Balance + account.Limit - sum < 0:
                    result.AddCommonMessage("Недостаточно средств");
                    break;
                case BankAccountType.DEBIT when account.Balance - sum < 0:
                    result.AddCommonMessage("Недостаточно средств");
                    break;
            }

            return result;
        }
    }

    public class TransactionException : Exception
    {
        public TransactionException(string Message) : base(Message)
        {

        }
    }

    public class BankAccountException : Exception
    {
        public BankAccountException(string Message) : base(Message)
        {

        }
    }
}
