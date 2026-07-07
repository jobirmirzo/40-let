using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.FoodId).HasColumnName("food_id");
        builder.Property(x => x.Quantity).HasColumnName("quantity");
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price");
        builder.Property(x => x.Discount).HasColumnName("discount");
        builder.Property(x => x.TotalPrice).HasColumnName("total_price");

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Food)
            .WithMany()
            .HasForeignKey(x => x.FoodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.FoodId);
    }
}
