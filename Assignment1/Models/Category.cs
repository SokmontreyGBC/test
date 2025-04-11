using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    [MaxLength(20, ErrorMessage = "Category name cannot be longer than 20 characters.")]
    public string CategoryName { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}