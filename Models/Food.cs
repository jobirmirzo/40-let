using _40Let.Enum;

namespace _40Let.Models;

public class Food
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public Category Category { get; set; }
    public string? Image { get; set; }
    public int Discount { get; set; }
    public bool HasDiscount { get; set; }
}