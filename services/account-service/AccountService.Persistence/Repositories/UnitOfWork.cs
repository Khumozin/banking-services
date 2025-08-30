using AccountService.Application.Contracts.Infrastructure;
using AccountService.Application.Contracts.Infrastructure.Kafka;
using AccountService.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace AccountService.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountServiceDbContext _context;
    private IAccountRepository _accountRepository;
    private readonly ICacheService _cacheService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<AccountRepository> _logger;

    public UnitOfWork(AccountServiceDbContext context, IAccountRepository accountRepository, ICacheService cacheService,
        IKafkaProducer kafkaProducer, ILogger<AccountRepository> logger)
    {
        _context = context;
        _accountRepository = accountRepository;
        _cacheService = cacheService;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public IAccountRepository AccountRepository =>
        _accountRepository ?? new AccountRepository(_context, _cacheService, _kafkaProducer, _logger);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}