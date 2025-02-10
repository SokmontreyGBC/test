namespace Assignment1.Models;

public class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    // navigation properties

    public ICollection<Product> Products { get; set; } = new List<Product>();
}