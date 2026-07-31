namespace _40Let.Features;

public class SuperAdminOptions
{
    public const string SectionName = "SuperAdmin";

    /// Comma-separated Telegram chat ids, e.g. "123456789,987654321".
    public string ChatIds { get; set; } = string.Empty;

    public bool IsSuperAdmin(long chatId) =>
        ChatIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(id => long.TryParse(id, out var parsed) && parsed == chatId);
}
