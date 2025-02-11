using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string? ProductDescription { get; set; }

    public decimal ProductPrice { get; set; }
    
    public int ProductStock { get; set; }

    public bool IsArchived { get; set; }

    // foreign key properties

    public int CategoryId { get; set; }

    // navigation properties

    public Category? Category { get; set; }

    public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
}