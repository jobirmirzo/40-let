namespace _40Let.Models;

/// <summary>
/// A single line of an order: one food, a quantity, and the price captured
/// at the moment of ordering (so later menu-price changes don't rewrite history).
/// </summary>
public class OrderItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long FoodId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int Discount { get; set; }
    public double TotalPrice { get; set; }

    public Order Order { get; set; } = null!;
    public Food Food { get; set; } = null!;
}
