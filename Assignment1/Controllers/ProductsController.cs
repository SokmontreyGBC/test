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
    public IActionResult Index()
    {
        //
        var inventory = _context.Products.ToList();
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
        // TODO Error message
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
        ViewBag.Categories = _context.Categories.Find(product.CategoryId);
        ViewBag.Quantity = _context.Stocks.Find(product.StockId);
        ViewBag.Price = _context.Products.FirstOrDefault(p =>p.ProductId == id );
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(int id,
        [Bind("ProductId,ProductName,ProductPrice,ProductDescription,CategoryId,StockId")] Product product)
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

    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
            
        }
        return NotFound();
    }
    
}