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
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
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

        ViewData["Categories"] = _context.Categories.ToList();

        ViewData["LowerStockThreshold"] = 10;

        return View(inventory);
    }

    [HttpGet]
    public IActionResult GetProducts(
        string orderType = "desc",
        string orderBy = "Name",
        string searchString = "",
        string selectedCategoriesString = "")
    {
        searchString = searchString.ToLower();
        var selectedCategories = selectedCategoriesString
            .Split(',');

        var inventory = _context.Products
            .Include(p => p.Category)
            .Where(p => String.IsNullOrWhiteSpace(searchString)
                        || p.ProductName.ToLower().Contains(searchString))
            .Where(p => String.IsNullOrWhiteSpace(selectedCategoriesString)
                        || selectedCategories.Contains(p.Category.CategoryName));

        Expression<Func<Product, object>> sortColumnSelector = orderBy switch
        {
            "ID" => p => p.ProductId,
            "Name" => p => p.ProductName,
            "Price" => p => p.ProductPrice,
            "Category" => p => p.Category.CategoryName,
            "ProductStock" => p => p.ProductStock,
            _ => p => p.ProductId
        };

        inventory = orderType.ToLower() == "desc"
            ? inventory.OrderByDescending(sortColumnSelector)
            : inventory.OrderBy(sortColumnSelector);

        var inventoryList = inventory.ToList();

        ViewData["OrderType"] = orderType;
        ViewData["LowerStockThreshold"] = 10;
        return PartialView("_ProductRows", inventoryList);
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
    [ValidateAntiForgeryToken]
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