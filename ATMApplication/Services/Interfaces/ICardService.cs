using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATMApplication.Models;
using ATMApplication.Validation;

namespace ATMApplication.Services
{
    public interface ICardService
    {
        public Task<ValidationResult> ValidateCard(CardEditModel model);

        public Task<CardEditModel> CreateCardForUser(User user, BankAccountType accountType, decimal moneyLimit = 0);

        public Task<CardEditModel> CreateCardForBankAccount(BankAccount account);

        public Task<Card> GetCardById(Guid cardId);

        public Task<IEnumerable<Card>> GetUserCards(string userId);

        public Task<IEnumerable<Card>> GetBankAccountCards(string accountId);

        public Task<BankAccount> GetCardBankAccount(string cardId);

        public Task BlockCard(Card card);

        public Task DepositWithdrawCash(string cardId, decimal sum, bool deposit = true);
    }
}
