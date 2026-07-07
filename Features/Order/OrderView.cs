using _40Let.Enum;

namespace _40Let.Features;

public class OrderView
{
    public Status Status { get; set; }
    public int Discount { get; set; }
    public bool HasPromoCode { get; set; }
    public double TotalPrice { get; set; }
}
