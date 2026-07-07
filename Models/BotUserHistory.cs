namespace _40Let.Models;

public class BotUserHistory
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long HistoryId { get; set; }
    public BotUser User { get; set; } = new();
    public Check Check { get; set; } = new();
}