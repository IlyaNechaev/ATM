using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Models
{
    public record BankAccount
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public decimal? Limit { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
        public Guid OwnerId { get; set; }

        public List<Card> Cards { get; set; }

        [Required]
        public BankAccountType BankAccountType { get; set; }
    }

    public enum BankAccountType { DEBIT, CREDIT }
}
