using _40Let.Enum;

namespace _40Let.Models;

public class Order
{
    public long Id { get; set; }
    public Status Status { get; set; }
    public int Discount { get; set; }
    public bool HasPromoCode { get; set; }
    public double TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
