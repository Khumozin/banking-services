using AccountService.Domain.Common;

namespace AccountService.Domain;

public class Transaction : BaseDomainEntity
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public Guid? SourceAccountId { get; set; }
    public Guid? DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "PENDING";
    public string TransactionType { get; set; } = string.Empty;
    public Account? SourceAccount { get; set; }
    public Account? DestinationAccount { get; set; }
}
