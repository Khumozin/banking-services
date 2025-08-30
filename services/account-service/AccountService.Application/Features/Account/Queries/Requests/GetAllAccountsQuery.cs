using AccountService.Application.DTOs;
using MediatR;

namespace AccountService.Application.Features.Account.Queries.Requests;

public class GetAllAccountsQuery : IRequest<List<AccountDTO>>
{
}