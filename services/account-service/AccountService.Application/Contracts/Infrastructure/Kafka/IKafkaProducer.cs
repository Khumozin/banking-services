namespace AccountService.Application.Contracts.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message);
}