namespace Artesanias.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductoRepository Productos { get; }
    IArtesanoRepository Artesanos { get; }
    ICartRepository Cart { get; }
    IOrderRepository Orders { get; }
    IUsuarioRepository Usuarios { get; }
    IPaymentTransactionRepository PaymentTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
