namespace ATMApplication.Models;

public struct CreateCardRequest
{
    public BankAccountType BankAccountType { get; set; }
}

public struct DepositWithdrawCashRequest
{
    public decimal Sum { get; set; }
    public ulong CardNumber { get; set; }
}

public struct TransactionRequest
{
    public ulong SourceCardNumber { get; set; }
    public ulong TargetCardNumber { get; set; }
    public decimal Sum { get; set; }
}

public struct BlockCardRequest
{
    public ulong CardNumber { get; set; }
}