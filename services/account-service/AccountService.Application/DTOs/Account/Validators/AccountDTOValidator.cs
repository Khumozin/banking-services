using System.Data;
using AccountService.Application.Contracts.Persistence;
using FluentValidation;

namespace AccountService.Application.DTOs.Validators;

public class AccountDTOValidator : BaseAccountValidator<AccountDTO>
{
    public AccountDTOValidator(IAccountRepository accountRepository)
        : base(accountRepository)
    {
        RuleFor(x => x.Balance)
            .GreaterThan(0)
            .WithMessage("Balance must be greater than 0");
    }
}