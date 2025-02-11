namespace Assignment1.Models;

public class Stock
{
    public int StockId { get; set; }

    public int StockQuantity { get; set; }

    // foreign key properties

    public int ProductId { get; set; }

    // navigation properties

    public Product Product { get; set; }
}