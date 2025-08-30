using AccountService.Application.Contracts.Infrastructure.Kafka;
using AccountService.Application.Contracts.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace AccountService.Application.Events;

public class TransactionEventConsumerService : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransactionEventConsumerService> _logger;

    public TransactionEventConsumerService(IKafkaConsumer kafkaConsumer, IServiceProvider serviceProvider,
        ILogger<TransactionEventConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken); // Wait for other services to start

        await _kafkaConsumer.ConsumeAsync("transaction.initiated", async (message) =>
        {
            try
            {
                var transactionInitiated = JsonConvert.DeserializeObject<TransactionInitiated>(message);
                if (transactionInitiated != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var accountService = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                    await accountService.ProcessTransactionAsync(transactionInitiated);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction initiated event: {Message}", message);
            }
        }, cancellationToken);
    }
}