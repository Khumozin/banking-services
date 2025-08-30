namespace AccountService.Application.Contracts.Infrastructure;

public interface ICacheService
{
    Task<decimal?> GetBalanceAsync(Guid accountId);
    Task SetBalanceAsync(Guid accountId, decimal balance);
    Task RemoveBalanceAsync(Guid accountId);
}