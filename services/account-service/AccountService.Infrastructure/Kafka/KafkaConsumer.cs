using AccountService.Application.Contracts.Infrastructure.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccountService.Infrastructure.Kafka;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly IConsumer<string, string> _consummer;
    private readonly ILogger<KafkaConsumer> _logger;


    public KafkaConsumer(IConfiguration config, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;

        ConsumerConfig consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "account-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
        };

        _consummer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public async Task ConsumeAsync(string topic, Func<string, Task> messageHandler,
        CancellationToken cancellationToken = default)
    {
        _consummer.Subscribe(topic);
        _logger.LogInformation("Started consuming from topic: {topic}", topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = _consummer.Consume(cancellationToken);
                    if (consumerResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Consumed message from topic {topic}: {message", topic,
                            consumerResult.Message.Value);

                        await messageHandler(consumerResult.Message.Value);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error consuming message from topic: {topic}", topic);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error processing message from topic: {topic}", topic);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consume cancelled for topic: {topic}", topic);
        }
        finally
        {
            _consummer.Close();
        }
    }
}