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

    [HttpPost]
    public IActionResult CreateOrder(List<OrderItem> cartItems)
    {
        try
        {
            // temp guest user
            var user = new User { UserId = -1, UserEmail = "Test", UserType = UserType.Guest };
            _context.Users.Add(user);
            _context.SaveChanges();
            // create the order first for an id
            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                UserId = -1,
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            _context.SaveChanges();
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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
        ViewBag.OrderItems = GetOrderItems();
        return View();
    }

    public User GetOrCreateUser(string email, string name)
    {
        // find by email
        var user = _context.Users
            .FirstOrDefault(u => u.UserEmail == email);
        if (user == null)
        {
            user = new User
            {
                UserEmail = email,
                UserName = name,
                UserType = UserType.Guest
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        return user;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckoutOrder(User userForm)
    {
        var cartItems = GetOrderItems();
        if (!ModelState.IsValid)
        {
            ViewBag.OrderItems = cartItems;
            return View(userForm);
        }

        var user = GetOrCreateUser(userForm.UserEmail, userForm.UserName ?? "Guest");
        var order = CreateOrder(user.UserId);
        var orderItems = CreateOrderItems(order.OrderId, cartItems);
        UpdateProductStock(orderItems);

        TempData["Name"] = user.UserName;
        return RedirectToAction("CheckoutConfirm", "Order");
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