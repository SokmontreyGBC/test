namespace Assignment1.Models;

public class Order
{
    public int OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }

    public User? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}