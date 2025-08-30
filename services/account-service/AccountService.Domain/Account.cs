using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AccountService.Domain.Common;

namespace AccountService.Domain;

public class Account : BaseDomainEntity
{
    [Key]
    [Column("account_id")]
    public Guid AccountId { get; set; } = Guid.NewGuid();
    
    [Column("owner_name")]
    [Required]
    [MaxLength(100)]
    public string OwnerName { get; set; } = string.Empty;
    
    [Column("balance", TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
}