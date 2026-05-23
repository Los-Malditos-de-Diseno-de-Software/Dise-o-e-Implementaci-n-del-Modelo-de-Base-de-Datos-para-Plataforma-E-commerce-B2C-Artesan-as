using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(u => u.Apellido).IsRequired().HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200).HasColumnType("NVARCHAR(200)");
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256).HasColumnType("NVARCHAR(256)");
        builder.Property(u => u.Rol).HasMaxLength(50).HasColumnType("NVARCHAR(50)");
        builder.Property(u => u.Telefono).HasMaxLength(20).HasColumnType("NVARCHAR(20)");
        builder.Property(u => u.IsDeleted).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(u => u.CreatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(u => u.UpdatedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");
        builder.Property(u => u.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(u => u.UpdatedAt).HasColumnType("DATETIME2");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.ToTable("Usuarios");
    }
}
