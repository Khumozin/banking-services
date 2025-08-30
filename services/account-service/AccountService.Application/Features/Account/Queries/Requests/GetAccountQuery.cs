using AccountService.Application.DTOs;
using MediatR;

namespace AccountService.Application.Features.Account.Queries.Requests;

public class GetAccountQuery : IRequest<AccountDTO>
{
    public Guid AccountId { get; set; }
}