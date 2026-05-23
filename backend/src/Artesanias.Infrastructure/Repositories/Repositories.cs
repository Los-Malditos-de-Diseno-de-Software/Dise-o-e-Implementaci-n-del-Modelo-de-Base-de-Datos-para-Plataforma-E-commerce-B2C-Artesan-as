using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using Artesanias.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Artesanias.Infrastructure.Repositories;

public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    public ProductoRepository(ArtesaniasDbContext context) : base(context) { }

    public async Task<IEnumerable<Producto>> GetAllWithImagesAsync(CancellationToken ct = default)
        => await _context.Productos
            .Include(p => p.Imagenes)
            .Include(p => p.Artesano)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<Producto?> GetByIdWithImagesAsync(Guid id, CancellationToken ct = default)
        => await _context.Productos
            .Include(p => p.Imagenes)
            .Include(p => p.Artesano)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

    public async Task<IEnumerable<Producto>> GetByArtesanoIdAsync(Guid artesanoId, CancellationToken ct = default)
        => await _context.Productos
            .Include(p => p.Imagenes)
            .Where(p => p.ArtesanoId == artesanoId && !p.IsDeleted)
            .ToListAsync(ct);

    public async Task<(IEnumerable<Producto> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _context.Productos
            .Include(p => p.Imagenes)
            .Include(p => p.Artesano)
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Nombre.Contains(search) ||
                p.Descripcion.Contains(search) ||
                p.Artesano.Nombre.Contains(search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}

public class ArtesanoRepository : Repository<Artesano>, IArtesanoRepository
{
    public ArtesanoRepository(ArtesaniasDbContext context) : base(context) { }

    public async Task<Artesano?> GetByIdWithProductosAsync(Guid id, CancellationToken ct = default)
        => await _context.Artesanos
            .Include(a => a.Productos.Where(p => !p.IsDeleted))
            .ThenInclude(p => p.Imagenes)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
}

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ArtesaniasDbContext context) : base(context) { }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Usuarios.AnyAsync(u => u.Email == email && !u.IsDeleted, ct);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ArtesaniasDbContext context) : base(context) { }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default)
        => await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Producto)
            .Include(o => o.Pago)
            .Include(o => o.Usuario)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
        => await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Pago)
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync(CancellationToken ct = default)
        => await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Pago)
            .Include(o => o.Usuario)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
}

public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ArtesaniasDbContext context) : base(context) { }

    public async Task<PaymentTransaction?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken ct = default)
        => await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.StripeSessionId == stripeSessionId, ct);

    public async Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        => await _context.PaymentTransactions
            .FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
}
