using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
 

    public int ProductId { get; set; }
    [Required]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; }
    [Required]
    [Display(Name = "Product Description")]
    public string ProductDescription { get; set; }

    public decimal ProductPrice { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }
    public int StockId { get; set; }
}