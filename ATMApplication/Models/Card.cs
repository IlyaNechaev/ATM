using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Models
{
    [Table("Card")]
    public record Card
    {
        [Key]
        public Guid Id { get; set; }
        public ulong CardNumber { get; set; }
        public DateTime MonthYear { get; set; }
        public string OwnerName { get; set; }
        public string HashPin { get; set; }
        public string HashCVV { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(AccountId))]
        public BankAccount Account { get; set; }
        public Guid AccountId { get; set; }
    }    
}
