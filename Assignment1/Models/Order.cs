using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string Address { get; set; } = null!;

    // Navigation Properties

    public ApplicationUser? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}