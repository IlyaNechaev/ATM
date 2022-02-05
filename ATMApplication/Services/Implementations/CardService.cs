using ATMApplication.Models;
using ATMApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using ATMApplication.Validation;

namespace ATMApplication.Services
{
    public class CardService : ICardService
    {
        IRepositoryFactory RepositoryFactory { get; set; }
        ILogger Logger { get; set; }
        ISecurityService SecurityService { get; set; }
        IMapper Mapper { get; set; }

        public CardService(IRepositoryFactory repositoryFactory,
                           ILogger<CardService> logger,
                           ISecurityService securityService,
                           IMapper mapper)
        {
            RepositoryFactory = repositoryFactory;
            Logger = logger;
            SecurityService = securityService;
            Mapper = mapper;
        }

        public async Task<ValidationResult> ValidateCard(CardEditModel model)
        {
            var validationResult = await ValidateCardEditModel(model);

            return validationResult;
        }

        public async Task<CardEditModel> CreateCardForUser(User user, CardType cardType)
        {
            var cardInfo = new CardEditModel
            {
                CardNumber = await GenerateCardNumber(),
                CVV = GenerateCVV(),
                MonthYear = GenerateMonthYear(),
                OnwerName = $"{user.FirstName.ToUpper()} {user.MiddleName.ToUpper()}"
            };

            var card = Mapper.Map<CardEditModel, Card>(cardInfo);

            card.Id = Guid.NewGuid();
            card.Owner = user;
            card.CardType = cardType;

            var CardRepository = RepositoryFactory.GetRepository<Card>();
            try
            {
                await CardRepository.AddAsync(card);
            }
            catch
            {
                return null;
            }

            return cardInfo;
        }

        public async Task<Card> GetCardById(Guid cardId)
        {
            var cardRepository = RepositoryFactory.GetRepository<Card>();
            Card card = null;

            try
            {
                card = await cardRepository.GetSingleAsync(card => card.Id.Equals(cardId));
            }
            catch (RepositoryException ex)
            {
                Logger?.LogError(ex.FullMessage);
            }

            return card;
        }

        public async Task<ICollection<Card>> GetUserCards(string userId)
        {
            var CardRepository = RepositoryFactory.GetRepository<Card>();
            var id = Guid.Parse(userId);

            var cards = await CardRepository.GetAsync(card => card.OwnerId.Equals(id));
            return cards.ToList();
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

        private string HashCVV(string cvv)
        {
            return SecurityService.GetPasswordHash(cvv);
        }
        private string HashPin(string pin)
        {
            return SecurityService.GetPasswordHash(pin);
        }
        private int GenerateCVV()
        {
            var random = new Random();
            return random.Next(100, 999);
        }
        private async Task<ValidationResult> ValidateCardEditModel(ICardValidationModel model)
        {
            var result = new ValidationResult();

            var CardRepository = RepositoryFactory.GetRepository<Card>();
            Card card = null;

            try
            {
                card = (await CardRepository.GetAsync(card => card.CardNumber == model.GetCardNumber())).Single();
            }
            catch { }

            if (card is null ||
                !HashCVV(model.GetCVV()).Equals(card.HashCVV) ||
                !HashPin(model.GetPin()).Equals(card.HashPin) ||
                !model.GetMonthYear().Equals(card.MonthYear))
            {
                result.AddCommonMessage("Карты не существует");
            }

            return result;
        }
    }
}
