using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AccountService.Domain.Common;

namespace AccountService.Domain;

public class Transaction : BaseDomainEntity
{
    [Key]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; } = Guid.NewGuid();

    [Column("source_account_id")]
    public Guid? SourceAccountId { get; set; }

    [Column("destination_account_id")]
    public Guid? DestinationAccountId { get; set; }

    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "PENDING";

    [Column("transaction_type")]
    [MaxLength(20)]
    public string TransactionType { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("SourceAccountId")]
    public Account? SourceAccount { get; set; }

    [ForeignKey("DestinationAccountId")]
    public Account? DestinationAccount { get; set; }
}
