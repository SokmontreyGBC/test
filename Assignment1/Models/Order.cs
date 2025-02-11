namespace Assignment1.Models;

public class Order
{
    public int OrderId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public DateTime OrderDate { get; set; }

    // foreign key properties

    public int CartId { get; set; }

    // navigation properties

    public Cart Cart { get; set; }
}