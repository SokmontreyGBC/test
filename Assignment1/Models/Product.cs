using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public decimal ProductPrice { get; set; }
    public int CategoryId { get; set; }
    public bool IsArchived { get; set; }
    public int ProductStock { get; set; }

    public Category? Category { get; set; }
}