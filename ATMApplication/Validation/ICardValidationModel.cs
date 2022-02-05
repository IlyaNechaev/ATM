using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Validation
{
    public interface ICardValidationModel
    {
        public string GetCVV() => string.Empty;

        public string GetPin() => string.Empty;

        public DateTime GetMonthYear() => DateTime.Now;

        public ulong GetCardNumber() => 0;
    }
}
