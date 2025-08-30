using AccountService.Application.Contracts.Persistence;
using AccountService.Application.DTOs.Common;
using FluentValidation;

namespace AccountService.Application.DTOs.Validators;

public class BaseAccountValidator<T> : AbstractValidator<T> where T : IBaseAccountDTO
{
    public BaseAccountValidator(IAccountRepository accountRepository)
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed {ComparisonValue} characters.");

        RuleFor(x => x.InitialBalance)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than zero.");
    }
}