using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATMApplication.Models;

namespace ATMApplication.Services
{
    public interface ICardService
    {
        public bool ValidateCVV(Card card, string cvv);

        public Task<Card> CreateCardForUser(User user, CardType cardType);

        public Task<Card> GetCard(Guid cardId);
    }
}
