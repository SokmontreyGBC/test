namespace Assignment1.Models;

public class Cart
{
    public int CartId { get; set; }

    public DateTime CartCreateAt { get; set; }

    public DateTime? CartExpiresAt { get; set; }

    public CartType CartType { get; set; }

    // foreign key properties

    public int? UserId { get; set; }

    public string? SessionId { get; set; }

    // navigation properties

    public User? User { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
