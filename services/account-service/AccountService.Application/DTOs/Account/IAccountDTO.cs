using AccountService.Application.DTOs.Common;

namespace AccountService.Application.DTOs;

public interface IAccountDTO : IBaseAccountDTO
{
    public Guid AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
}