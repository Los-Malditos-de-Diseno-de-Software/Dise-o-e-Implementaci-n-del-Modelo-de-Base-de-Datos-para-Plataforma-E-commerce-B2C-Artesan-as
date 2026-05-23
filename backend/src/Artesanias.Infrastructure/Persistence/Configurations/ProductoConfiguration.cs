using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(p => p.ArtesanoId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150).HasColumnType("NVARCHAR(150)");
        builder.Property(p => p.Descripcion).HasMaxLength(1000).HasColumnType("NVARCHAR(1000)");
        builder.Property(p => p.Precio).HasColumnType("DECIMAL(10,2)").IsRequired();
        builder.Property(p => p.Stock).HasColumnType("INT").IsRequired();
        builder.Property(p => p.EsUnico).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(p => p.IsDeleted).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(p => p.CreatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(p => p.UpdatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(p => p.UpdatedAt).HasColumnType("DATETIME2");

        builder.HasMany(p => p.Imagenes)
               .WithOne(i => i.Producto)
               .HasForeignKey(i => i.ProductoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Productos");
    }
}
