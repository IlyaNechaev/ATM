using ATMApplication.Models;
using ATMApplication.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services.Substitute
{
    public class CardServiceSubstitute : ICardService
    {
        public Task<CardEditModel> CreateCardForUser(User user)
        {
            var newCard = new CardEditModel
            {
                CardNumber = GenerateCardNumber(),
                MonthYear = DateTime.Now.AddDays(30),
                OnwerName = $"{user.FirstName} {user.MiddleName}",
                CVV = GenerateCVV()
            };

            return Task.FromResult(newCard);
        }

        public async Task<Card> GetCardById(Guid cardId)
        {
            var card = new Card()
            {
                CardNumber = GenerateCardNumber(),
                Id = Guid.NewGuid(),
                MonthYear = GenerateMonthYear(),
                OwnerName = $"Имя Фамилия"
            };

            return await Task.FromResult(card);
        }

        public async Task<IEnumerable<Card>> GetUserCards(string userId)
        {
            var cardsCount = new Random().Next(1, 5);
            ICollection<Card> cards = new List<Card>(cardsCount);

            for (; cardsCount > 0; cardsCount--)
            {
                cards.Add(new Card()
                {
                    CardNumber = GenerateCardNumber(),
                    Id = Guid.NewGuid(),
                    MonthYear = GenerateMonthYear(),
                    OwnerName = $"Имя Фамилия"
                });
            }

            return await Task.FromResult(cards);
        }

        public string HashCVV(string cvv)
        {
            throw new NotImplementedException();
        }

        public string HashPin(string pin)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateCard(CardEditModel model)
        {
            throw new NotImplementedException();
        }

        private DateTime GenerateMonthYear()
        {
            var date = DateTime.Now.AddYears(4);
            return new DateTime(date.Year, date.Month + 1, 1);
        }

        private ulong GenerateCardNumber()
        {
            ulong cardNumber = 0;
            var rand = new Random();

            for (int i = 0; i < 16; i++)
            {
                cardNumber += (ulong)(rand.Next(0, 9) * Math.Pow(10, i));
            }

            return cardNumber;
        }

        private int GenerateCVV()
        {
            return new Random().Next(100, 999);
        }

        public Task<IEnumerable<Card>> GetBankAccountCards(string accountId)
        {
            throw new NotImplementedException();
        }

        public Task<CardEditModel> CreateCardForBankAccount(BankAccount account)
        {
            throw new NotImplementedException();
        }

        public Task BlockCard(Card card)
        {
            throw new NotImplementedException();
        }

        public Task<CardEditModel> CreateCardForUser(User user, BankAccountType accountType, decimal moneyLimit = 0)
        {
            throw new NotImplementedException();
        }

        public Task<BankAccount> GetCardBankAccount(string cardId)
        {
            throw new NotImplementedException();
        }

        public Task DepositWithdrawCash(Card card, decimal sum, bool deposit = true)
        {
            throw new NotImplementedException();
        }

        public Task DepositWithdrawCash(string cardId, decimal sum, bool deposit = true)
        {
            throw new NotImplementedException();
        }
    }
}
