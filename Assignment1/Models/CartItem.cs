namespace Assignment1.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int CartItemQuantity { get; set; }
    
    // foreign key properties

    public int ProductId { get; set; }

    // navigation properties

    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}