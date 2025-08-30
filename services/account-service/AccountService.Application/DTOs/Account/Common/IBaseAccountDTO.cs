namespace AccountService.Application.DTOs.Common;

public interface IBaseAccountDTO
{
    public string OwnerName { get; set; }
    public decimal InitialBalance { get; set; }
}