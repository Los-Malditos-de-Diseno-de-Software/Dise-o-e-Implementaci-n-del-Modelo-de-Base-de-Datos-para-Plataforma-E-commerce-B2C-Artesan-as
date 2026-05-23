using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(c => c.SessionId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(c => c.UsuarioId).HasColumnType("UNIQUEIDENTIFIER").IsRequired(false);
        builder.Property(c => c.UltimaActualizacion).HasColumnType("DATETIME2");

        builder.HasIndex(c => c.SessionId).IsUnique();

        builder.HasMany(c => c.Items)
               .WithOne(i => i.ShoppingCart)
               .HasForeignKey(i => i.ShoppingCartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Usuario)
               .WithMany(u => u.Carritos)
               .HasForeignKey(c => c.UsuarioId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.ToTable("ShoppingCarts");
    }
}

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(i => i.ShoppingCartId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(i => i.ProductoId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(i => i.Cantidad).HasColumnType("INT").IsRequired();
        builder.Property(i => i.PrecioUnitarioCongelado).HasColumnType("DECIMAL(10,2)").IsRequired();

        builder.HasOne(i => i.Producto)
               .WithMany(p => p.CartItems)
               .HasForeignKey(i => i.ProductoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("CartItems");
    }
}
