using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class CartController: Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();

        cart.ForEach(i => i.Product = _context.Products.Find(i.ProductId));

        ViewBag.TotalCart = TotalCart();

        return PartialView("_CartDetails", cart);
    }

    public decimal TotalCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();

        decimal total = 0;
        foreach (var item in cart)
        {
            var product = _context.Products.Find(item.ProductId);
            if (product == null) continue;
            total += product.ProductPrice * item.Quantity;
        }

        return total;
    }

}