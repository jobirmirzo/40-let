using _40Let.Enum;

namespace _40Let.Features;

public class OrderView
{
    public Status Status { get; set; }
    public int Discount { get; set; }
    public bool HasPromoCode { get; set; }
    public double TotalPrice { get; set; }

    public List<OrderItemView> Items { get; set; } = [];
}

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
