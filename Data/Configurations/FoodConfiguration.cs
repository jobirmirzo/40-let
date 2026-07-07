using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _40Let.Data.Configurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("foods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Price).HasColumnName("price");
        builder.Property(x => x.Category)
            .HasColumnName("category")
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(x => x.Image).HasColumnName("image");
        builder.Property(x => x.Discount).HasColumnName("discount");
        builder.Property(x => x.HasDiscount).HasColumnName("has_discount");

        builder.HasIndex(x => x.Category);
    }
}
