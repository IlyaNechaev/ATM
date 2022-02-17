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
        public Task<CardEditModel> CreateCardForUser(User user, CardType cardType)
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
                CardType = CardType.DEBIT,
                Id = Guid.NewGuid(),
                Owner = null,
                MonthYear = GenerateMonthYear(),
                OwnerName = $"Имя Фамилия"
            };

            return await Task.FromResult(card);
        }

        public async Task<ICollection<Card>> GetUserCards(string userId)
        {
            var cardsCount = new Random().Next(1, 5);
            ICollection<Card> cards = new List<Card>(cardsCount);

            for (; cardsCount > 0; cardsCount--)
            {
                cards.Add(new Card()
                {
                    CardNumber = GenerateCardNumber(),
                    CardType = CardType.DEBIT,
                    Id = Guid.NewGuid(),
                    Owner = null,
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
    }
}
