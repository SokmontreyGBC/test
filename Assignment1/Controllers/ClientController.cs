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

    [HttpGet]
    public IActionResult UpdateCartQuantity(int id, int quantity)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            TempData["Success"] = false;
            TempData["Message"] = "Product not found.";
            return RedirectToAction("Index");
        }

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

        if (quantity > product.ProductStock)
        {
            TempData["Success"] = false;
            TempData["Message"] = $"You can't have more than {product.ProductStock} of {product.ProductName} in your cart.";
            return RedirectToAction("Index");
        }

        if (quantity <= 0)
        {
            TempData["Success"] = false;
            TempData["Message"] = $"You can't have zero items. Please delete the item instead.";
            return RedirectToAction("Index");
        }

        cartItem.Quantity = quantity;
        StashCart(cart);
        TempData["Success"] = true;
        TempData["Message"] = $"Successfully added {quantity} of {product.ProductName} to cart.";
        return RedirectToAction("Index");
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