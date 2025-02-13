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
        return View(clientInventory);
    }
    

    public IActionResult AddToCart(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
        
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
}