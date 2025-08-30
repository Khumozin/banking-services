using AccountService.Application.Contracts.Infrastructure;
using StackExchange.Redis;

namespace AccountService.Infrastructure.Cache;

public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public CacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
    }
    
    public async Task<decimal?> GetBalanceAsync(Guid accountId)
    {
        var key = $"balance:{accountId}";
        var value = await _database.StringGetAsync(key);

        if (value.HasValue && decimal.TryParse(value.ToString(), out decimal balance))
        {
            return balance;
        }
        
        return null;
    }

    public async Task SetBalanceAsync(Guid accountId, decimal balance)
    {
        var key = $"balance:{accountId}";
        await _database.StringSetAsync(key, balance.ToString(), TimeSpan.FromMinutes(15));
    }

    public async Task RemoveBalanceAsync(Guid accountId)
    {
        var key = $"balance:{accountId}";
        await _database.KeyDeleteAsync(key);
    }
}