using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class BotUserHistoryConfiguration : IEntityTypeConfiguration<BotUserHistory>
{
    public void Configure(EntityTypeBuilder<BotUserHistory> builder)
    {
        builder.ToTable("bot_user_histories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.HistoryId).HasColumnName("history_id");

        builder.HasOne(x => x.User)
            .WithMany(x => x.BotUserHistory)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Check)
            .WithMany(x => x.BotUserHistory)
            .HasForeignKey(x => x.HistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.HistoryId }).IsUnique();
    }
}
