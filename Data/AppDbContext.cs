using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BotUser> BotUsers => Set<BotUser>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<BotUserOrder> BotUserOrders => Set<BotUserOrder>();
    public DbSet<Check> Checks => Set<Check>();
    public DbSet<BotUserHistory> BotUserHistories => Set<BotUserHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies every IEntityTypeConfiguration<T> found in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
