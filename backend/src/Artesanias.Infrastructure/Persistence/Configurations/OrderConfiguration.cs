using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(o => o.UsuarioId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(o => o.Total).HasColumnType("DECIMAL(10,2)").IsRequired();
        builder.Property(o => o.EstadoPedido).HasMaxLength(50).HasColumnType("NVARCHAR(50)");
        builder.Property(o => o.DireccionEnvio).HasMaxLength(500).HasColumnType("NVARCHAR(500)");
        builder.Property(o => o.IsDeleted).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(o => o.CreatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(o => o.UpdatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(o => o.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(o => o.UpdatedAt).HasColumnType("DATETIME2");

        builder.HasOne(o => o.Usuario)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Pago)
               .WithOne(p => p.Order)
               .HasForeignKey<PaymentTransaction>(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Orders");
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(i => i.OrderId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(i => i.ProductoId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(i => i.Cantidad).HasColumnType("INT").IsRequired();
        builder.Property(i => i.PrecioUnitario).HasColumnType("DECIMAL(10,2)").IsRequired();

        // Computed property - ignored in DB
        builder.Ignore(i => i.Subtotal);

        builder.HasOne(i => i.Producto)
               .WithMany(p => p.OrderItems)
               .HasForeignKey(i => i.ProductoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("OrderItems");
    }
}
