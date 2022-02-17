using System.ComponentModel.DataAnnotations;

namespace ATMApplication.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Sum { get; set; }
        public DateTime TransactionTime { get; set; }
        public BankAccount AccountSender { get; set; }
        public BankAccount AccountReceiver { get; set; }
    }
}
