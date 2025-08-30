using System.IO.Pipes;
using System.Net.Http.Json;
using AccountService.Application.Contracts.Infrastructure.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountService.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = "account-service-producer"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        try
        {
            string json = JsonConvert.SerializeObject(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            };
            
            await _producer.ProduceAsync(topic, kafkaMessage);
            _logger.LogInformation("Message sent to topic {topic}: {message}", topic, json);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send message to topic: {topic}", topic);
            throw;
        }
    }
    
    public void Dispose()
    {
        _producer?.Dispose();
    }
}