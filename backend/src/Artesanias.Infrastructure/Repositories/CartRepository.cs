using Artesanias.Domain.Entities;
using Artesanias.Domain.Interfaces;
using Artesanias.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Artesanias.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ArtesaniasDbContext _context;

    public CartRepository(ArtesaniasDbContext context) => _context = context;

    public async Task<ShoppingCart?> GetBySessionIdAsync(Guid sessionId, CancellationToken ct = default)
        => await _context.ShoppingCarts
            .Include(c => c.Items).ThenInclude(i => i.Producto).ThenInclude(p => p.Imagenes)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, ct);

    public async Task<ShoppingCart> GetOrCreateBySessionIdAsync(Guid sessionId, CancellationToken ct = default)
    {
        var cart = await GetBySessionIdAsync(sessionId, ct);
        if (cart is null)
        {
            cart = new ShoppingCart { SessionId = sessionId };
            await _context.ShoppingCarts.AddAsync(cart, ct);
        }
        return cart;
    }

    public async Task DeleteCartAndItemsAsync(Guid sessionId, CancellationToken ct = default)
    {
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, ct);

        if (cart is not null)
            _context.ShoppingCarts.Remove(cart);
    }

    public async Task DeleteByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, ct);

        if (cart is not null)
            _context.ShoppingCarts.Remove(cart);
    }

    // Alias corto para compatibilidad con los handlers de Application
    public Task<ShoppingCart> GetOrCreateBySessionAsync(Guid sessionId, CancellationToken ct = default)
        => GetOrCreateBySessionIdAsync(sessionId, ct);

    public async Task AddCartAsync(ShoppingCart cart, CancellationToken ct = default)
        => await _context.ShoppingCarts.AddAsync(cart, ct);

    public async Task AddCartItemAsync(CartItem item, CancellationToken ct = default)
        => await _context.CartItems.AddAsync(item, ct);

    public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productoId, CancellationToken ct = default)
        => await _context.CartItems
            .FirstOrDefaultAsync(i => i.ShoppingCartId == cartId && i.ProductoId == productoId, ct);

    public void RemoveCartItem(CartItem item)
        => _context.CartItems.Remove(item);
}
