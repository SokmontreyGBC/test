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
    public IActionResult Index(string selectedCategoryString = "")
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Where(p => !p.IsArchived)
            .ToList();

        ViewData["Columns"] = new List<string>
        {
            "ID",
            "Name",
            "Description",
            "Price",
            "Category",
            "ProductStock"
        };

        ViewData["OrderableColumns"] = new List<string>
        {
            "ID",
            "Name",
            "Price",
            "Category",
            "ProductStock"
        };

        ViewData["Categories"] = _context.Categories.ToList();
        ViewData["LowerStockThreshold"] = 10;
        ViewData["SelectedCategoryString"] = selectedCategoryString;

        return View(products);
    }

    [HttpGet]
    public IActionResult DeleteCartItem(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            TempData["Success"] = false;
            TempData["Message"] = "Product not found.";
            return RedirectToAction("Index");
        }
        var cartJson = HttpContext.Session.GetString("Cart") ?? "[]";
        var cart = JsonSerializer.Deserialize<List<OrderItem>>(cartJson);
        var cartItem = cart.Find(oi => oi.ProductId == product.ProductId);
        if (cartItem != null)
        {
            cart.Remove(cartItem);
            StashCart(cart);
        }
        TempData["Success"] = true;
        TempData["Message"] = $"Successfully removed {product.ProductName} from cart.";
        return RedirectToAction("Index");
    }

    private void StashCart(List<OrderItem> cart)
    {
        string cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("Cart", cartJson);
    }
    
}