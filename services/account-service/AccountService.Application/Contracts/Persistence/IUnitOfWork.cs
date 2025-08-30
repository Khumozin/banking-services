namespace AccountService.Application.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    IAccountRepository AccountRepository { get; }
    Task SaveAsync();
}