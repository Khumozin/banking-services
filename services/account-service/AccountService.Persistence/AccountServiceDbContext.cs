using AccountService.Domain;
using AccountService.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Persistence;

public class AccountServiceDbContext : DbContext
{
    public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountServiceDbContext).Assembly);

        // Configure Account entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.OwnerName);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.SourceAccount)
                .WithMany()
                .HasForeignKey(e => e.SourceAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DestinationAccount)
                .WithMany()
                .HasForeignKey(e => e.DestinationAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SourceAccountId);
            entity.HasIndex(e => e.DestinationAccountId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseDomainEntity>())
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}