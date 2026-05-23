using Artesanias.Domain.Entities;

namespace Artesanias.Domain.Interfaces;

public interface IProductoRepository : IRepository<Producto>
{
    Task<IEnumerable<Producto>> GetAllWithImagesAsync(CancellationToken ct = default);
    Task<Producto?> GetByIdWithImagesAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Producto>> GetByArtesanoIdAsync(Guid artesanoId, CancellationToken ct = default);
    Task<(IEnumerable<Producto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
}

public interface IArtesanoRepository : IRepository<Artesano>
{
    Task<Artesano?> GetByIdWithProductosAsync(Guid id, CancellationToken ct = default);
}

public interface ICartRepository
{
    Task<ShoppingCart?> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default);
    Task<ShoppingCart> GetOrCreateBySessionIdAsync(Guid sessionId, CancellationToken ct = default);
    // Alias usado por los handlers (nombre más corto)
    Task<ShoppingCart> GetOrCreateBySessionAsync(Guid sessionId, CancellationToken ct = default);
    Task DeleteCartAndItemsAsync(Guid sessionId, CancellationToken ct = default);
    Task DeleteByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task AddCartAsync(ShoppingCart cart, CancellationToken ct = default);
    Task AddCartItemAsync(CartItem item, CancellationToken ct = default);
    Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productoId, CancellationToken ct = default);
    void RemoveCartItem(CartItem item);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetAllWithDetailsAsync(CancellationToken ct = default);
}

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken ct = default);
    Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
