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

        public Task<CardEditModel> CreateCardForUser(User user, CardType cardType);

        public Task<Card> GetCardById(Guid cardId);

        public Task<ICollection<Card>> GetUserCards(string userId);
    }
}
