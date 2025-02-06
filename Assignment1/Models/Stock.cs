namespace Assignment1.Models;

public class Stock
{
    public Stock(int stockId, int stockQuantity)
    {
        StockId = stockId;
        StockQuantity = stockQuantity;
    }

    public int StockId { get; set; }

    public int StockQuantity { get; set; }
}
