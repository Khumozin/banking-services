using AccountService.Application.DTOs;
using AccountService.Application.Events;
using AccountService.Domain;

namespace AccountService.Application.Contracts.Persistence;

public interface IAccountRepository
{
    Task<Account> CreateAccountAsync(CreateAccountDTO request);
    Task<Account> GetAccountAsync(Guid accountId);
    Task<IReadOnlyList<Account>> GetAllAccountsAsync();
    Task<bool> UpdateBalanceAsync(Guid accountId, decimal newBalance);
    Task<bool> ProcessTransactionAsync(TransactionInitiated transaction);
}