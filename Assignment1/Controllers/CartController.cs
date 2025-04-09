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
            message = $"Successfully added {quantity} of {product.ProductName} to cart."
        });
    }

    private void StashCart(List<OrderItem> cart)
    {
        string cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", cartJson);
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