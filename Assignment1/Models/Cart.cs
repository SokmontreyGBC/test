namespace Assignment1.Models;

public class Cart
{
    public int CartId { get; set; }

    public CartType CartType { get; set; }

    // foreign key properties

    public string? SessionId { get; set; }

    // navigation properties

    public User? User { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
