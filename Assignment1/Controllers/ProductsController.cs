using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;
/*
 *  This is the employee inventory controller
 *  It is responsible for non-client facing actions
 */
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Index(string searchString,string sort)
    {
        
        var inventory = _context.Products.Include(p => p.Category).ToList();
        if (!string.IsNullOrEmpty(searchString))
        {
            inventory = inventory.Where
                (s => s.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
             
        }
        ViewData["nameSort"] = string.IsNullOrEmpty(sort) ? "name_dsc" : "";
        ViewData["priceSort"] = string.IsNullOrEmpty(sort) ? "price_dsc" : "";
        ViewData["stockSort"] = string.IsNullOrEmpty(sort) ? "stock_dsc" : "";
        ViewData["categorySort"] = string.IsNullOrEmpty(sort) ? "category_dsc" : "";

        switch (sort)
        {
            case "name_dsc":
                inventory = inventory.OrderByDescending(s => s.ProductName).ToList();
                 break;
            case "price_asc":
                inventory = inventory.OrderBy(s => s.ProductPrice).ToList();
                break;
            case "price_dsc":
                inventory = inventory.OrderByDescending(s => s.ProductPrice).ToList();
                break;
        
            case "stock_dsc":
                inventory = inventory.OrderByDescending(s => s.ProductStock).ToList();
                break;
            case "category_dsc":
                inventory = inventory.OrderByDescending(s => s.Category.CategoryName).ToList();
                break;
            default:
                inventory = inventory.OrderBy(s => s.ProductName).ToList();
                break;
        }

        return View(inventory);
        
    }

    
    
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }   
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.Price = _context.Products.FirstOrDefault(p => p.ProductId == id);
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(int id,
        [Bind("ProductId,ProductName,ProductPrice,ProductDescription,CategoryId,ProductStock")] Product product)
    {
        if (id != product.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                _context.SaveChanges();
            }
            catch
                (DbUpdateConcurrencyException)
            {
                if (!ProductsExist(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return RedirectToAction("Index");
        
    }
    public bool ProductsExist(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound();

        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]

    public IActionResult DeleteConfirmed(int productid)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == productid);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return NotFound();
    }
    
    
}