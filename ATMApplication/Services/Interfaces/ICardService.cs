using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATMApplication.Models;

namespace ATMApplication.Services
{
    public interface ICardService
    {
        public bool ValidateCard(CardEditModel model);

        public Task<Card> CreateCardForUser(User user, CardType cardType);

        public Task<Card> GetCardById(Guid cardId);
    }
}
