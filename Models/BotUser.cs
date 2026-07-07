namespace _40Let.Models;

public class BotUser
{
    public long Id { get; set; }
    public string? Fullname { get; set; }
    public string? PhoneNumber { get; set; }
    public long ChatId { get; set; }
    public string? Role { get; set; }
    public ICollection<BotUserHistory>? BotUserHistory { get; set; }
}