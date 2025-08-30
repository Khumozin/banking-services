namespace AccountService.Application.DTOs;

public class CreateAccountDTO : ICreateAccountDTO
{
    public string OwnerName { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = decimal.Zero;
}