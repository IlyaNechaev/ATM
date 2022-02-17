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

        public async Task TransferMoney(BankAccount source, BankAccount dest, decimal sum)
        {
            var validationResult = ValidateBankAccountForTransaction(source, sum);
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
            source.Money -= sum;
            dest.Money += sum;
            await BankAccountRepository.UpdateAsync(source);
            await BankAccountRepository.UpdateAsync(dest);

            await TransactionRepository.UpdateAsync(transaction);
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
                Money = 0.0m,
                AccountNumber = await accountNumberTask
            };

            await BankAccountRepository.AddAsync(account);

            return account;
        }

        public async Task AddNewCard(BankAccount account, Card card)
        {
            var BankAccountRepository = RepositoryFactory.GetRepository<BankAccount>();

            if (account.Cards.Any(card => card.IsActive))
            {
                throw new BankAccountException($"Невозможно привязать новую карту к счету {account.AccountNumber}, поскольку к нему привязаны активные карты");
            }

            account.Cards.Add(card);

            await BankAccountRepository.UpdateAsync(account);
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

        private ValidationResult ValidateBankAccountForTransaction(BankAccount account, decimal sum)
        {
            var result = new ValidationResult();
            switch (account.BankAccountType)
            {
                case BankAccountType.CREDIT when account.Money + account.Limit - sum < 0:
                    result.AddCommonMessage("Недостаточно средств");
                    break;
                case BankAccountType.DEBIT when account.Money - sum < 0:
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
