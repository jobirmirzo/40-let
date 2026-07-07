using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class CheckConfiguration : IEntityTypeConfiguration<Check>
{
    public void Configure(EntityTypeBuilder<Check> builder)
    {
        builder.ToTable("checks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.Withdrawal).HasColumnName("withdrawal");
        builder.Property(x => x.OrderedCount).HasColumnName("ordered_count");
        builder.Property(x => x.WithPromoCode).HasColumnName("with_promo_code");
        builder.Property(x => x.DiscountedAmount).HasColumnName("discounted_amount");
        builder.Property(x => x.Discount).HasColumnName("discount");
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrderId);
    }
}
