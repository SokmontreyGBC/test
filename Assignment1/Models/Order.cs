namespace Assignment1.Models;

public class Order
{
    public Order(int orderId, int orderQuantity, DateTime orderDate, int productId, int userId)
    {
        OrderId = orderId;
        OrderQuantity = orderQuantity;
        OrderDate = orderDate;
        ProductId = productId;
        UserId = userId;
    }

    public int OrderId { get; set; }

    public int OrderQuantity { get; set; }

    public DateTime OrderDate { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }
}