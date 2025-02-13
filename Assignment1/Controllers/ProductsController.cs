using System.Globalization;
using System.Linq.Expressions;
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
    public IActionResult Index()
    {
        var inventory = _context.Products
            .Include(p => p.Category)
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

        return View(inventory);
    }

    [HttpGet]
    public IActionResult GetProducts(string orderType = "desc", string orderBy = "Name")
    {
        var inventory = _context.Products
            .Include(p => p.Category)
            .ToList();

        inventory = orderBy switch
        {
            "ID" => orderType == "desc"
                ? inventory.OrderBy(p => p.ProductId).ToList()
                : inventory.OrderByDescending(p => p.ProductId).ToList(),
            "Name" => orderType == "desc"
                ? inventory.OrderBy(p => p.ProductName).ToList()
                : inventory.OrderByDescending(p => p.ProductName).ToList(),
            "Price" => orderType == "desc"
                ? inventory.OrderBy(p => p.ProductPrice).ToList()
                : inventory.OrderByDescending(p => p.ProductPrice).ToList(),
            "Category" => orderType == "desc"
                ? inventory.OrderBy(p => p.Category.CategoryName).ToList()
                : inventory.OrderByDescending(p => p.Category.CategoryName).ToList(),
            "ProductStock" => orderType == "desc"
                ? inventory.OrderBy(p => p.ProductStock).ToList()
                : inventory.OrderByDescending(p => p.ProductStock).ToList(),
            _ => orderType == "desc"
                ? inventory.OrderBy(p => p.ProductName).ToList()
                : inventory.OrderByDescending(p => p.ProductName).ToList()
        };

        ViewData["OrderType"] = orderType;
        return PartialView("_ProductRows", inventory);
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
        [Bind("ProductId,ProductName,ProductPrice,ProductDescription,CategoryId,ProductStock")]
        Product product)
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