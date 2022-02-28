using System.ComponentModel.DataAnnotations;

namespace ATMApplication.Models
{
    public record Transaction
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public decimal Sum { get; set; }
        public DateTime TransactionTime { get; set; }
        public BankAccount AccountSender { get; set; }
        public BankAccount AccountReceiver { get; set; }
    }
}
