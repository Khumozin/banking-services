namespace AccountService.Application.Contracts.Infrastructure.Kafka;

public interface IKafkaConsumer
{
    Task ConsumeAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);
}