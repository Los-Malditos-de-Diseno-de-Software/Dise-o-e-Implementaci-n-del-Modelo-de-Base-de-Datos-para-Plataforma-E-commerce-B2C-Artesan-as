using Artesanias.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Artesanias.Infrastructure.Persistence.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("UNIQUEIDENTIFIER").ValueGeneratedNever();
        builder.Property(p => p.OrderId).HasColumnType("UNIQUEIDENTIFIER").IsRequired();
        builder.Property(p => p.MetodoPago).HasMaxLength(50).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.EstadoPago).HasMaxLength(50).HasColumnType("NVARCHAR(50)");
        builder.Property(p => p.ReferenciaPasarela).HasMaxLength(200).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.PayloadPasarela).HasMaxLength(4000).HasColumnType("NVARCHAR(4000)");
        builder.Property(p => p.StripeSessionId).HasMaxLength(200).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.StripePaymentIntentId).HasMaxLength(200).HasColumnType("NVARCHAR(200)");
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(p => p.UpdatedAt).HasColumnType("DATETIME2");

        builder.HasIndex(p => p.StripeSessionId);

        builder.ToTable("PaymentTransactions");
    }
}
