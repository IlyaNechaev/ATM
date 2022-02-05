using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Models
{
    public class CardViewModel
    {
        public string Id { get; set; }
        public ulong CardNumber { get; set; }
        public DateTime MonthYear { get; set; }
        public string OwnerName { get; set; }
        public string CardType { get; set; }
        public UserViewModel Owner { get; set; }
    }
}
