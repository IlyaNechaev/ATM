namespace ATMApplication.Models;

public class CreateCardRequest
{
    public string UserId { get; set; }
    public BankAccountType BankAccountType { get; set; }
}