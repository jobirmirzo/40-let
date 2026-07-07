namespace _40Let.Models;

public class BotUserOrder
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public BotUser User { get; set; } = new();
    public Order Order { get; set; } = new();
}