using ATMApplication.Models;
using ATMApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ATMApplication.Services
{
    public class CardService : ICardService
    {
        IRepositoryFactory RepositoryFactory { get; set; }
        ILogger Logger { get; set; }

        public CardService(IRepositoryFactory repositoryFactory,
                           ILogger logger)
        {
            RepositoryFactory = repositoryFactory;
            Logger = logger;
        }
        public bool ValidateCVV(Card card, string cvv)
        {
            card.
        }

        public async Task<Card> CreateCardForUser(User user, CardType cardType)
        {
            return new Card
            {
                Id = Guid.NewGuid(),
                Owner = user,
                OwnerName = $"{user.FirstName.ToUpper()} {user.MiddleName.ToUpper()}",
                CardType = cardType,
                CardNumber = await GenerateCardNumber(),
                MonthYear = GenerateMonthYear()
            };
        }

        public async Task<Card> GetCard(Guid cardId)
        {
            var cardRepository = RepositoryFactory.GetRepository<Card>();
            Card card = null;

            try
            {
                card = await cardRepository.GetSingleAsync(card => card.Id.Equals(cardId));
            }
            catch (RepositoryException ex)
            {
                Logger.LogError(ex.FullMessage);
            }

            return card;
        }

        private async Task<ulong> GenerateCardNumber()
        {
            var cardRepository = RepositoryFactory.GetRepository<Card>();

            ulong cardNumber = 0;
            var rand = new Random();

            do
            {
                for (int i = 0; i < 16; i++)
                {
                    cardNumber += (ulong)(rand.Next(0, 9) * Math.Pow(10, i));
                }
            }
            // Если карта с подобным номером уже зарегистрирована, то создаем новый номер
            while ((await cardRepository.GetAsync(card => card.CardNumber.Equals(cardNumber))) is not null);

            return cardNumber;
        }

        private DateTime GenerateMonthYear()
        {
            var date = DateTime.Now.AddYears(4);
            return new DateTime(date.Year, date.Month + 1, 1);
        }

    }
}
