using AccountService.Application.Contracts.Persistence;
using AccountService.Application.DTOs;
using AccountService.Application.Features.Account.Queries.Requests;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Account.Queries.Handlers;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDTO>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public GetAccountQueryHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountDTO> Handle(GetAccountQuery query, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetAccountAsync(query.AccountId);
        return _mapper.Map<AccountDTO>(account);
    }
}