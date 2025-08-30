using AccountService.Application.Contracts.Persistence;
using AccountService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextFactory<AccountServiceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        return services;
    }
}