using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class BotUserConfiguration : IEntityTypeConfiguration<BotUser>
{
    public void Configure(EntityTypeBuilder<BotUser> builder)
    {
        builder.ToTable("bot_users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Fullname).HasColumnName("full_name").HasMaxLength(200);
        builder.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
        builder.Property(x => x.ChatId).HasColumnName("chat_id");
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);

        builder.HasIndex(x => x.ChatId).IsUnique();
        builder.HasIndex(x => x.PhoneNumber);
    }
}
