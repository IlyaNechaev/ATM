using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Extensions
{
    public static class CardExtensions
    {
        public static string GetName(this BankAccountType accountType)
        {
            return accountType switch
            {
                BankAccountType.CREDIT => "Кредитовый",
                BankAccountType.DEBIT => "Дебетовый",
                _ => throw new NotImplementedException()
            };
        }
    }
}
