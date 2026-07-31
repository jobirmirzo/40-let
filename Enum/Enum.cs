using System.Text.Json.Serialization;

namespace _40Let.Enum;

/// Stored case-insensitively (see BotUserConfiguration) so pre-existing
/// lowercase "admin"/"user" rows keep working. Serialised as a string, not a
/// number, so the frontend's simple string checks (role === 'admin') keep
/// working without needing to know numeric codes.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    User = 1,
    Admin = 2,
    SuperAdmin = 3
}

public enum Status
{
    InProgress = 1,
    InPayment = 2,
    Canceled = 3,
    PaymentDone = 4,
    Cooking = 5,
    InDelivery = 6,
    Delivered = 7,
}

public enum Category
{
    Food = 1,
    Drink = 2
}