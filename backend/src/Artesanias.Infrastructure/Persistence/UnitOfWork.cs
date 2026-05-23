using Artesanias.Domain.Interfaces;
using Artesanias.Infrastructure.Persistence;
using Artesanias.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Artesanias.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ArtesaniasDbContext _context;
    private IDbContextTransaction? _transaction;

    private IProductoRepository? _productos;
    private IArtesanoRepository? _artesanos;
    private ICartRepository? _cart;
    private IOrderRepository? _orders;
    private IUsuarioRepository? _usuarios;
    private IPaymentTransactionRepository? _paymentTransactions;

    public UnitOfWork(ArtesaniasDbContext context) => _context = context;

    public IProductoRepository Productos
        => _productos ??= new ProductoRepository(_context);

    public IArtesanoRepository Artesanos
        => _artesanos ??= new ArtesanoRepository(_context);

    public ICartRepository Cart
        => _cart ??= new CartRepository(_context);

    public IOrderRepository Orders
        => _orders ??= new OrderRepository(_context);

    public IUsuarioRepository Usuarios
        => _usuarios ??= new UsuarioRepository(_context);

    public IPaymentTransactionRepository PaymentTransactions
        => _paymentTransactions ??= new PaymentTransactionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
