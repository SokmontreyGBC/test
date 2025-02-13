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
        ViewBag.Cart = GetCart();
        return View(clientInventory);
    }

    

    public IActionResult AddToCart(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }
        /*
         * check for active cart
         * add the product to the list
         * save the list back to the session
         */
        List<Product> cart = GetCart();
        cart.Add(product);
        StashCart(cart);
        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }
        List<Product> cart = GetCart();
        var toRemove = cart.Find(p => p.ProductId == id);
        if (toRemove != null)
        {
            cart.Remove(toRemove);
            StashCart(cart);
        }
        // gross ajax stuff to stop the refresh problem with offcanvas
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }
        return RedirectToAction("Index");
    }

    
    // CART SESSION COOKING
    private List<Product> GetCart()
    {
        string cartJson = HttpContext.Session.GetString("Cart"); // why is the syntax for this so verbose lmao
        // make empty list if !cart
        if(string.IsNullOrEmpty(cartJson))
        {
            return new List<Product>();
        }
        else
        {
            return JsonSerializer.Deserialize<List<Product>>(cartJson); // I just threw up in my mouth
        }
    }
    
    private void StashCart(List<Product> cart)
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
        var order = new Order{UserId = user.UserId};
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
        var inventory = _context.OrderItems.Where(oi=>oi.OrderId==orderId).ToList();
       
        return View("OrderCheckout",inventory);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CheckoutOrder(Order order)
    {
       

        return View();
    }
}