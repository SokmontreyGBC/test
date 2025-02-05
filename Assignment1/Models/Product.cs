using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
    public int ProductId { get; set; }
    
    [Required]
    public string ProductName { get; set; }
    
    
}