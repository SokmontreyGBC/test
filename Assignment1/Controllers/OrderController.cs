using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

//[Route ("[controller]/[action]")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<OrderController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
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
            .Where(item => item.Product != null)
            .ToList();
    }

    [HttpGet]
    public IActionResult CheckoutOrder()
    {
        try
        {
            if (HttpContext.Session.GetString("Cart") == null || HttpContext.Session.GetString("Cart") == "[]")
            {
                return RedirectToAction("Index", "Client");
            }

            // TODO update this base on _userManager;
            ViewBag.User = User;

            return View(GetOrderItems());
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("Index", "Client");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Add this to prevent CSRF attacks
    public async Task<IActionResult> CheckoutConfirm(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            return BadRequest("Address is required");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        // Check if cart is empty
        var cartItems = GetOrderItems();
        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index", "Client");
        }

        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            OrderStatus = OrderStatus.Pending,
            UserId = user.Id,
            Address = address
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var orderItems = CreateOrderItems(order.OrderId, cartItems);
        UpdateProductStock(orderItems);

        HttpContext.Session.Remove("Cart");
        return View(order);
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
        try
        {
            var orderList = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();
            return View(orderList);
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("ServerError", "Error");
        }
    }
}