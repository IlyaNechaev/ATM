using ATMApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    interface IBankService
    {
        public void TransferMoney(Card source, Card dest);
    }
}
