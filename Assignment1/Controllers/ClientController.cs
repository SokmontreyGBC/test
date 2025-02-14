using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

/*
 *  This is the client inventory controller
 */
public class ClientController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();

        foreach (var item in cart)
        {
            item.Product = _context.Products.Find(item.ProductId);
        }

        return PartialView("_CartRows", cart);
    }

    [HttpGet]
    public JsonResult AddToCart(int id, int quantity)
    {
        var product = _context.Products.Find(id);
        if (product == null)
            return new JsonResult(new
            {
                success = false,
                message = "Product not found."
            });

        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();
        var cartItem = cart.Find(oi => oi.ProductId == product.ProductId);

        if (cartItem == null)
        {
            cartItem = new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = 0
            };
            cart.Add(cartItem);
        }

        var newQuantity = cartItem.Quantity + quantity;

        if (newQuantity > product.ProductStock)
            return new JsonResult(new
            {
                success = false,
                message = $"You can't have more than {product.ProductStock} of {product.ProductName} in your cart."
            });

        if (newQuantity <= 0)
            return new JsonResult(new
            {
                success = false,
                message = $"You can't have zero items. Please delete the item instead."
            });

        cartItem.Quantity = newQuantity;
        StashCart(cart);
        return new JsonResult(new
        {
            success = true,
            message = $"Successfully added {cartItem.Quantity} of {product.ProductName} to cart."
        });
    }

    [HttpGet]
    public IActionResult UpdateCartQuantity(int id, int quantity)
    {
        var product = _context.Products.Find(id);
        if (product == null)
            return RedirectToAction("Index");

        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();
        var cartItem = cart.Find(oi => oi.ProductId == product.ProductId);

        if (cartItem == null)
        {
            cartItem = new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = 0
            };
            cart.Add(cartItem);
        }

        cartItem.Quantity = quantity;

        if (cartItem.Quantity > product.ProductStock)
        {
            cartItem.Quantity = product.ProductStock;
            StashCart(cart);
            return RedirectToAction("Index");
        }

        StashCart(cart);
        return RedirectToAction("Index");
    }

    private void StashCart(List<OrderItem> cart)
    {
        string cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", cartJson);
    }

    public IActionResult CheckoutOrder()
    {
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();

        var user = new User
        {
            UserName = "admin",
            UserEmail = "admin@admin.com",
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        var order = new Order { UserId = user.UserId };
        _context.Orders.Add(order);
        _context.SaveChanges();
        int orderId = order.OrderId;
        foreach (var item in cart)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = orderId, ProductId = item.ProductId, Quantity = item.Quantity
            });
        }

        _context.SaveChanges();
        var inventory = _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();

        return View("OrderCheckout", inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckoutOrder(Order order)
    {
        return View();
    }
}