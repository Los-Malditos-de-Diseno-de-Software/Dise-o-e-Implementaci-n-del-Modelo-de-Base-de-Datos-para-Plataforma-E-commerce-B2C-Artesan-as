using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class ArtesanoConfiguration : IEntityTypeConfiguration<Artesano>
{
    public void Configure(EntityTypeBuilder<Artesano> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(a => a.Nombre).IsRequired().HasMaxLength(150).HasColumnType("NVARCHAR(150)");
        builder.Property(a => a.HistoriaBiografia).HasMaxLength(2000).HasColumnType("NVARCHAR(2000)");
        builder.Property(a => a.ComunidadOrigen).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(a => a.CreatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(a => a.UpdatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(a => a.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(a => a.UpdatedAt).HasColumnType("DATETIME2");
        builder.Property(a => a.IsDeleted).HasColumnType("BIT").HasDefaultValue(false);

        builder.HasMany(a => a.Productos)
               .WithOne(p => p.Artesano)
               .HasForeignKey(p => p.ArtesanoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Artesanos");
    }
}
