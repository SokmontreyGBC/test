using System.Text.Json;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;
/*
 *  This is the client inventory controller
 */
public class InventoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var clientInventory = _context.Products.Include(p => p.Category).ToList();
        // ViewBag.Cart = GetCart();
        return View(clientInventory);
    }

    // public IActionResult AddToCart(int id)
    // {
    //     var product = _context.Products.Find(id);
    //     if (product == null)
    //     {
    //         return NotFound();
    //     }
    //     /*
    //      * check for active cart
    //      * add the product to the list
    //      * save the list back to the session
    //      */
    //     List<Product> cart = GetCart();
    //     cart.Add(product);
    //     StashCart(cart);
    //     return RedirectToAction("Index");
    // }

    // public IActionResult RemoveFromCart(int id)
    // {
    //     var product = _context.Products.Find(id);
    //     if (product == null)
    //     {
    //         return NotFound();
    //     }
    //     List<Product> cart = GetCart();
    //     var toRemove = cart.Find(p => p.ProductId == id);
    //     if (toRemove != null)
    //     {
    //         cart.Remove(toRemove);
    //         StashCart(cart);
    //     }
    //     // gross ajax stuff to stop the refresh problem with offcanvas
    //     if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
    //     {
    //         return Json(new { success = true });
    //     }
    //     return RedirectToAction("Index");
    // }

    [HttpGet]
    public IActionResult UpdateCartQuantity(int productId, int updateDirection)
    {
        var product = _context.Products.Find(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>();

        if (cart.All(oi => oi.ProductId != productId))
        {
            cart.Add(new OrderItem {
                ProductId = productId,
                Quantity = 0
            });
        }

        var cartItem = cart.Find(oi => oi.ProductId == productId);
        cartItem.Quantity += updateDirection;

        if (cartItem.Quantity  < 0) cartItem.Quantity = 0;
        else if (cartItem.Quantity > product.ProductStock) cartItem.Quantity = product.ProductStock;

        StashCart(cart);
        return Content($"{cartItem.Quantity}");
    }

    private void StashCart(List<OrderItem> cart)
    {
        string cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", cartJson);
    }

    // CART SESSION COOKING
    // private List<Product> GetCart()
    // {
    //     string cartJson = HttpContext.Session.GetString("Cart"); // why is the syntax for this so verbose lmao
    //     // make empty list if !cart
    //     if(string.IsNullOrEmpty(cartJson))
    //     {
    //         return new List<Product>();
    //     }
    //     else
    //     {
    //         return JsonSerializer.Deserialize<List<Product>>(cartJson); // I just threw up in my mouth
    //     }
    // }
}