using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Required]
    [Display(Name = "Product Name")]

    public string ProductName { get; set; }
    public string? ProductDescription { get; set; }
    [Required]
    [Display(Name = "Price")]
    public decimal ProductPrice { get; set; }
    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    public bool IsArchived { get; set; }
    [Required]
    [Display(Name = "Stock")]
    [Range(0, int.MaxValue)]
    public int ProductStock { get; set; }

    public Category? Category { get; set; }
}
