using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Artesanias.Infrastructure.Persistence;

public class ArtesaniasDbContext : DbContext
{
    public ArtesaniasDbContext(DbContextOptions<ArtesaniasDbContext> options) : base(options) { }

    public DbSet<Artesano> Artesanos => Set<Artesano>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtesaniasDbContext).Assembly);

        // Global query filter: Soft Delete
        modelBuilder.Entity<Artesano>().HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Producto>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Entity<Usuario>().HasQueryFilter(u => !u.IsDeleted);
    }
}
