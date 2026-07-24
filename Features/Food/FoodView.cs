using _40Let.Enum;

namespace _40Let.Features;

public class FoodView
{
    public string? Name { get; set; }
    public double Price { get; set; }
    public Category Category { get; set; }
    public int Discount { get; set; }
    public IFormFile? ImageFile { get; set; }
    public bool HasDiscount { get; set; }
}
