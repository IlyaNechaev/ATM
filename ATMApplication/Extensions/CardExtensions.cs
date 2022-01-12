using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Extensions
{
    public static class CardExtensions
    {
        public static string GetName(this CardType cardType)
        {
            return cardType switch
            {
                CardType.CREDIT => "Кредитовая",
                CardType.DEBIT => "Дебетовая",
                _ => throw new NotImplementedException()
            };
        }
    }
}
