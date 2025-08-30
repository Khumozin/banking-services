using AccountService.Application.Contracts.Persistence;
using AccountService.Application.DTOs;
using AccountService.Application.DTOs.Validators;
using AccountService.Application.Features.Account.Commands.Requests;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Account.Commands.Handlers;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDTO>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountDTO> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var response = new AccountDTO();
        var validator = new CreateAccountDTOValidator(_unitOfWork.AccountRepository);
        var validationResult = await validator.ValidateAsync(command.AccountDto);

        if (!validationResult.IsValid)
        {
            return response;
        }

        var account = await _unitOfWork.AccountRepository.CreateAccountAsync(command.AccountDto);
        await _unitOfWork.SaveAsync();
        
        response = _mapper.Map<AccountDTO>(account);

        return response;
    }
}