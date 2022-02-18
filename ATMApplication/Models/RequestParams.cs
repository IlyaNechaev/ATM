namespace ATMApplication.Models;

public class CreateCardRequest
{
    public string UserId { get; set; }
    public BankAccountType BankAccountType { get; set; }
}

public record DepositWithdrawCash
{
    public decimal Sum { get; set; }
    public string CardId { get; set; }
}