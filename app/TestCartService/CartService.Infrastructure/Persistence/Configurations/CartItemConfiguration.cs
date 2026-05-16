using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartService.Infrastructure.Persistence.Configurations;

internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItem");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ProductId)
            .IsRequired();

        builder.Property(c => c.Quantity)
            .IsRequired();

        builder.Property(c => c.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.CreatedAt)
            .IsRequired();
    }
}