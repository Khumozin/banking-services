using AccountService.Application.Contracts.Infrastructure;
using AccountService.Application.Contracts.Infrastructure.Kafka;
using AccountService.Infrastructure.Cache;
using AccountService.Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AccountService.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(connectionString ?? "localhost:6379");
        });
        
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IKafkaConsumer, KafkaConsumer>();
        services.AddScoped<IKafkaProducer, KafkaProducer>();
        
        return services;
    }
}