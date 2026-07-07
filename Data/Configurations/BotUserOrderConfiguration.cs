using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class BotUserOrderConfiguration : IEntityTypeConfiguration<BotUserOrder>
{
    public void Configure(EntityTypeBuilder<BotUserOrder> builder)
    {
        builder.ToTable("bot_user_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // A given order belongs to a user only once.
        builder.HasIndex(x => new { x.UserId, x.OrderId }).IsUnique();
    }
}
