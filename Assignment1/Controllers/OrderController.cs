using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckoutOrder(User userForm)
    {
        var orderItems = GetOrderItems();
        if (!ModelState.IsValid)
        {
            ViewBag.OrderItems = orderItems;
            return View(userForm);
        }



        return RedirectToAction("Index", "Client");
    }
}