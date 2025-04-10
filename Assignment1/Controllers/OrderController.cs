using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    private List<OrderItem> GetOrderItems()
    {
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();
        var products = _context.Products.ToList();
        return cart.Join(products,
                oi => oi.ProductId,
                p => p.ProductId,
                (oi, p) => new OrderItem
                {   
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    ProductId = p.ProductId,
                    Quantity = oi.Quantity,
                    Product = p
                })
            .ToList();
    }

    [HttpGet]
    public IActionResult CheckoutOrder()
    {
        if (HttpContext.Session.GetString("Cart") == null || HttpContext.Session.GetString("Cart") == "[]")
        {
            return RedirectToAction("Index", "Client");
        }
        ViewBag.OrderItems = GetOrderItems();
        return View();
    }

    public Order CreateOrder(int userId)
    {
        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            OrderStatus = OrderStatus.Pending,
            UserId = userId
        };
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }
    
    [HttpGet]
    public IActionResult CheckoutConfirm()
    {
        ViewBag.OrderItems = GetOrderItems();
        HttpContext.Session.Remove("Cart");
        return View();
    }

    public List<OrderItem> CreateOrderItems(int orderId, List<OrderItem> cartItems)
    {
        var orderItems = cartItems.Select(oi => new OrderItem
        {
            OrderId = orderId,
            ProductId = oi.ProductId,
            Quantity = oi.Quantity
        }).ToList();
        _context.OrderItems.AddRange(orderItems);
        _context.SaveChanges();
        return orderItems;
    }

    public void UpdateProductStock(List<OrderItem> orderItems)
    {
        orderItems.ForEach(oi =>
        {
            var product = _context.Products.Find(oi.ProductId);
            if (product == null) return;
            product.ProductStock -= oi.Quantity;
            _context.Products.Update(product);
        });
        _context.SaveChanges();
    }

    [HttpGet]
    public IActionResult AllOrders()
    {
        var orderList = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();
        return View(orderList);
    }
}