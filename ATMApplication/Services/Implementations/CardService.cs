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
using System.Text;

namespace ATMApplication.Services
{
    public class CardService : ICardService
    {
        IRepositoryFactory RepositoryFactory { get; set; }
        ILogger Logger { get; set; }
        ISecurityService SecurityService { get; set; }
        IMapper Mapper { get; set; }
        IBankService BankService { get; set; }

        public CardService(IRepositoryFactory repositoryFactory,
                           ILogger<CardService> logger,
                           ISecurityService securityService,
                           IBankService bankService,
                           IMapper mapper)
        {
            RepositoryFactory = repositoryFactory;
            Logger = logger;
            SecurityService = securityService;
            Mapper = mapper;
            BankService = bankService;
        }

        public async Task<ValidationResult> ValidateCard(CardEditModel model)
        {
            var validationResult = await ValidateCardEditModel(model);

            return validationResult;
        }

        public async Task<CardEditModel> CreateCardForUser(User user, BankAccountType accountType, decimal moneyLimit = 0)
        {
            var account = await BankService.CreateBankAccountForUser(user, accountType, moneyLimit);
            var cardInfo = await CreateCardForBankAccount(account);

            return cardInfo;
        }

        public async Task<CardEditModel> CreateCardForBankAccount(BankAccount account)
        {
            var accountOwner = account.Owner;
            if (accountOwner == null)
            {
                var UserRepository = RepositoryFactory.GetRepository<User>();
                accountOwner = await UserRepository.GetSingleAsync(user => user.Id == account.OwnerId);
            }

            var cardInfo = new CardEditModel
            {
                CardNumber = await GenerateCardNumber(),
                CVV = GenerateCVV(),
                Pin = GeneratePIN(),
                MonthYear = GenerateMonthYear(),
                OnwerName = $"{accountOwner.FirstName.ToUpper()} {accountOwner.MiddleName.ToUpper()}"
            };

            var card = Mapper.Map<CardEditModel, Card>(cardInfo);

            card.Id = Guid.NewGuid();
            card.Account = account;

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

        public async Task<IEnumerable<Card>> GetUserCards(string userId)
        {
            var id = Guid.Parse(userId);
            var cards = new List<Card>();

            var accounts = await BankService.GetUserBankAccounts(userId);
            foreach(var account in accounts)
            {
                var accountCards = await GetBankAccountCards(account.Id.ToString());
                foreach (var card in accountCards)
                {
                    cards.Add(card);
                }                
            }

            return cards;
        }

        private async Task<ulong> GenerateCardNumber()
        {
            var cardRepository = RepositoryFactory.GetRepository<Card>();

            ulong cardNumber = 0;
            var rand = new Random();

            do
            {
                cardNumber = 0;
                for (int i = 0; i < 16; i++)
                {
                    cardNumber += (ulong)(rand.Next(0, 9) * Math.Pow(10, i));
                }
            }
            // Если карта с подобным номером уже зарегистрирована, то создаем новый номер
            while ((await cardRepository.GetAsync(card => card.CardNumber.Equals(cardNumber)))?.Count() > 0);

            return cardNumber;
        }

        public async Task<IEnumerable<Card>> GetBankAccountCards(string accountId)
        {
            var CardRepository = RepositoryFactory.GetRepository<Card>();
            var cards = await CardRepository.GetAsync(card => card.AccountId == Guid.Parse(accountId));

            return cards;
        }

        public async Task BlockCard(Card card)
        {
            var CardRepository = RepositoryFactory.GetRepository<Card>();
            card.IsActive = false;

            await CardRepository.UpdateAsync(card);
        }

        public async Task<BankAccount> GetCardBankAccount(string cardId)
        {
            var CardRepository = RepositoryFactory.GetRepository<Card>();

            var card = await CardRepository.GetSingleAsync(card => card.Id == Guid.Parse(cardId), nameof(Card.Account));

            return card.Account;
        }

        public async Task DepositWithdrawCash(string cardId, decimal sum, bool deposit = true)
        {
            var account = await GetCardBankAccount(cardId.ToString());

            try
            {
                if (sum < 0)
                    throw new TransactionException("Сумма перевода не может быть отрицательной");
                await BankService.TransferMoney(account, sum, deposit);
            }
            catch (TransactionException)
            {
                throw;
            }
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
        private string GeneratePIN()
        {
            var pin = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < 4; i++)
            {
                pin.Append(random.Next(0, 9));
            }
            return pin.ToString();
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
