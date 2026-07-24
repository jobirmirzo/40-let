using _40Let.Enum;

namespace _40Let.Features;

public class OrderView
{
    public Status Status { get; set; }
    public int Discount { get; set; }
    public bool HasPromoCode { get; set; }
    public double TotalPrice { get; set; }

    /// <summary>
    /// The basket lines. Saved with the order on create; on update they replace
    /// the existing lines, unless the list is empty (then the lines are kept).
    /// </summary>
    public List<OrderItemView> Items { get; set; } = [];
}

/// <summary>
/// One basket line. <see cref="UnitPrice"/> is captured client-side at ordering
/// time so later menu-price changes don't rewrite history.
/// </summary>
public class OrderItemView
{
    public long FoodId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int Discount { get; set; }
    public double TotalPrice { get; set; }
}

public class OrderStatusView
{
    public Status Status { get; set; }
}
