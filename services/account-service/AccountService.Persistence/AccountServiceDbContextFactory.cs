using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AccountService.Persistence;

public class AccountServiceDbContextFactory : IDesignTimeDbContextFactory<AccountServiceDbContext>
{
    public AccountServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AccountServiceDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("PostgreSQL");
        builder.UseNpgsql(connectionString);

        return new AccountServiceDbContext(builder.Options);
    }
}