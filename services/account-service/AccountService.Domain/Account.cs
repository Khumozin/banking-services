using AccountService.Domain.Common;

namespace AccountService.Domain;

public class Account : BaseDomainEntity
{
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public string OwnerName { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
}