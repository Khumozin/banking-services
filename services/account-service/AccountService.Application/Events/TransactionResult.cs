namespace AccountService.Application.Events;

public class TransactionResult
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // COMPLETED or FAILED
    public string? Reason { get; set; }
}