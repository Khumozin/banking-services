using AccountService.Application.DTOs.Common;

namespace AccountService.Application.DTOs;

public class AccountDTO : IAccountDTO
{
    public Guid AccountId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = decimal.Zero;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}