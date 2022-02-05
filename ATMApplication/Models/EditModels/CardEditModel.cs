using ATMApplication.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Models
{
    public class CardEditModel : ICardValidationModel
    {
        public ulong CardNumber { get; set; }
        public string OnwerName { get; set; }
        public DateTime MonthYear { get; set; }
        public int CVV { get; set; }
        public string Pin { get; set; }

        public DateTime GetMonthYear() => MonthYear; 
        public string GetCVV() => CVV.ToString();
        public string GetPin() => Pin;
        public ulong GetCardNumber() => CardNumber;
    }
}
