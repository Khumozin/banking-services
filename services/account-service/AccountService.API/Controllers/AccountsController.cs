using AccountService.Application.DTOs;
using AccountService.Application.Features.Account.Commands.Requests;
using AccountService.Application.Features.Account.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : Controller
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAllAccounts()
    {
        var accounts = await _mediator.Send(new GetAllAccountsQuery());
        return Ok(accounts);
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<AccountDTO>> GetAccount(Guid accountId)
    {
        var account = await _mediator.Send(new GetAccountQuery { AccountId = accountId });
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDTO>> CreateAccout([FromBody] CreateAccountDTO createAccountDTO)
    {
        var command = new CreateAccountCommand { AccountDto = createAccountDTO };
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{accountId}/balance")]
    public async Task<ActionResult<decimal>> GetAccountBalance(Guid accountId)
    {
        var account = await _mediator.Send(new GetAccountQuery { AccountId = accountId });
        return Ok(new { accountId = account.AccountId, balance = account.Balance });
    }

    [HttpGet("health")]
    public ActionResult HealthCheck()
    {
        return Ok(new { status = "OK", service = "accounts-service", timestamp = DateTime.UtcNow });
    }
}