namespace _40Let.Models;

public class Check
{
   public long Id { get; set; }
   public long OrderId { get; set; }
   public double Withdrawal { get; set; }
   public long OrderedCount { get; set; }
   public bool WithPromoCode { get; set; }
   public double DiscountedAmount { get; set; }
   public int Discount { get; set; }
   public DateTime CreatedAt { get; set; }

   public Order Order { get; set; } = null!;
   public ICollection<BotUserHistory>? BotUserHistory { get; set; }
}
