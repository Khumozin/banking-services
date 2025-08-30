using AccountService.Application.DTOs;
using MediatR;

namespace AccountService.Application.Features.Account.Commands.Requests;

public class CreateAccountCommand : IRequest<AccountDTO>
{
    public CreateAccountDTO AccountDto { get; set; }
}