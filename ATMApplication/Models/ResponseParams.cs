namespace ATMApplication.Models;

public record TransactionResponse
{
    public CardViewModel SourceCard { get; set; }
    public CardViewModel DestinationCard { get; set;}
    public decimal Sum { get; set; }
    public DateTime TransactionTime { get; set; }
}
