using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
    public Product(int productId, string productName, string productDescription, decimal productPrice, int categoryId, int stockId)
    {
        ProductId = productId;
        ProductName = productName;
        ProductDescription = productDescription;
        ProductPrice = productPrice;
        CategoryId = categoryId;
        StockId = stockId;
    }

    public int ProductId { get; set; }
    
    public string ProductName { get; set; }

    public string ProductDescription { get; set; }

    public decimal ProductPrice { get; set; }

    public int CategoryId { get; set; }

    public int StockId { get; set; }
}