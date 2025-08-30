using AccountService.Application.Contracts.Infrastructure;
using AccountService.Application.Contracts.Infrastructure.Kafka;
using AccountService.Application.Contracts.Persistence;
using AccountService.Application.DTOs;
using AccountService.Application.Events;
using AccountService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountServiceDbContext _context;
    private readonly ILogger<AccountRepository> _logger;
    private readonly ICacheService _cacheService;
    private readonly IKafkaProducer _kafkaProducer;

    public AccountRepository(AccountServiceDbContext context, ICacheService cacheService, IKafkaProducer kafkaProducer,
        ILogger<AccountRepository> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task<Account> CreateAccountAsync(CreateAccountDTO request)
    {
        var account = new Account
        {
            OwnerName = request.OwnerName,
            Balance = request.InitialBalance
        };

        _context.Add(account);
        await _context.SaveChangesAsync();

        // Cache the initial balance
        await _cacheService.SetBalanceAsync(account.AccountId, account.Balance);

        _logger.LogInformation("Account created: {AccountId} for {OwnerName}", account.AccountId, account.OwnerName);

        return account;
    }

    public async Task<Account> GetAccountAsync(Guid accountId)
    {
        var cachedBalance = await _cacheService.GetBalanceAsync(accountId);
        if (cachedBalance.HasValue)
        {
            var cachedAccount = _context.Accounts
                .AsNoTracking()
                .FirstOrDefault(x => x.AccountId == accountId);

            if (cachedAccount != null)
            {
                cachedAccount.Balance = cachedBalance.Value;
                return cachedAccount;
            }
        }

        // Fallback to database
        var account = await _context.Accounts
            .FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (account != null)
        {
            await _cacheService.SetBalanceAsync(account.AccountId, account.Balance);
        }

        return account;
    }

    public async Task<IReadOnlyList<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateBalanceAsync(Guid accountId, decimal newBalance)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (account == null) return false;

        account.Balance = newBalance;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _cacheService.SetBalanceAsync(account.AccountId, account.Balance);

        return true;
    }

    public async Task<bool> ProcessTransactionAsync(TransactionInitiated transaction)
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var transactionId = Guid.Parse(transaction.TransactionId);
            var result = new TransactionResult()
            {
                TransactionId = transaction.TransactionId,
                Status = "FAILED"
            };

            if (transaction.TransactionType == "DEPOSIT")
            {
                // Handle deposit
                var destinationAccount = await GetAccountAsync(Guid.Parse(transaction.DestinationAccountId!));
                if (destinationAccount == null)
                {
                    result.Reason = "Destination account not found";
                }
                else
                {
                    await UpdateBalanceAsync(destinationAccount.AccountId,
                        destinationAccount.Balance + transaction.Amount);
                    result.Status = "COMPLETED";
                }
            }
            else if (transaction.TransactionType == "TRANSFER")
            {
                // Handle transfer
                var sourceAccount = await GetAccountAsync(Guid.Parse(transaction.SourceAccountId!));
                var destinationAccount = await GetAccountAsync(Guid.Parse(transaction.DestinationAccountId!));

                if (sourceAccount == null)
                {
                    result.Reason = "Source account not found";
                }
                else if (destinationAccount == null)
                {
                    result.Reason = "Destination account not found";
                }
                else if (sourceAccount.Balance < transaction.Amount)
                {
                    result.Reason = "Insufficient funds";
                }
                else
                {
                    await UpdateBalanceAsync(sourceAccount.AccountId,
                        sourceAccount.Balance - transaction.Amount);
                    await UpdateBalanceAsync(destinationAccount.AccountId,
                        destinationAccount.Balance + transaction.Amount);
                    result.Status = "COMPLETED";
                }
            }

            await dbTransaction.CommitAsync();

            // Publish result event
            var eventName = result.Status == "COMPLETED" ? "transaction.completed" : "transaction.failed";
            await _kafkaProducer.ProduceAsync(eventName, result);

            _logger.LogInformation("Transaction {TransactionId} processed with status {Status}",
                transaction.TransactionId, result.Status);

            return result.Status == "COMPLETED";
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(ex, "Error processing transaction {TransactionId}", transaction.TransactionId);

            await _kafkaProducer.ProduceAsync("transaction.failed", new TransactionResult()
            {
                TransactionId = transaction.TransactionId,
                Status = "FAILED",
                Reason = "Internal error"
            });

            return false;
        }
    }
}