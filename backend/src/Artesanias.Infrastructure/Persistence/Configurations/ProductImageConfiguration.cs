using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(i => i.ProductoId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        // VARBINARY(MAX) para almacenar la imagen binaria de la artesanía
        builder.Property(i => i.ImageData).HasColumnType("VARBINARY(MAX)").IsRequired();
        builder.Property(i => i.ContentType).HasMaxLength(50).HasColumnType("NVARCHAR(50)");
        builder.Property(i => i.EsPrincipal).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(i => i.CreatedAt).HasColumnType("DATETIME2");

        builder.ToTable("ProductImages");
    }
}
