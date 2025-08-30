using AccountService.Application.Contracts.Persistence;
using AccountService.Application.DTOs;
using AccountService.Application.Features.Account.Queries.Requests;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Account.Queries.Handlers;

public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountDTO>>
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;

    public GetAllAccountsQueryHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<List<AccountDTO>> Handle(GetAllAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllAccountsAsync();
        
        return _mapper.Map<List<AccountDTO>>(accounts);
    }
}