namespace AccountService.Application.Events;

public class TransactionInitiated
{
    public string TransactionId { get; set; } = string.Empty;
    public string? SourceAccountId { get; set; }
    public string? DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty; // DEPOSIT or TRANSFER
}