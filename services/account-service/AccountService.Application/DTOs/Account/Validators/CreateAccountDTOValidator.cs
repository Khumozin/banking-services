using AccountService.Application.Contracts.Persistence;
using FluentValidation;

namespace AccountService.Application.DTOs.Validators;

public class CreateAccountDTOValidator : BaseAccountValidator<CreateAccountDTO>
{
    public CreateAccountDTOValidator(IAccountRepository accountRepository)
        : base(accountRepository)
    {
    }
}