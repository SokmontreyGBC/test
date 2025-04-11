using System.Linq.Expressions;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Engines;


namespace Assignment1.Controllers;
//[Route ("[controller]/[action]")]

public class ProductController: Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetProducts(
        string orderType = "desc",
        string orderBy = "Name",
        string searchString = "",
        string selectedCategoriesString = "",
        bool isAdmin = false) // TODO: handle this when authentication is implemented
    {
        searchString = searchString.ToLower();
        var selectedCategories = selectedCategoriesString
            .Split(',');
        try
        {
            var inventory = _context.Products
                .Include(p => p.Category)
                .Where(p => String.IsNullOrWhiteSpace(searchString)
                            || p.ProductName.ToLower().Contains(searchString))
                .Where(p => String.IsNullOrWhiteSpace(selectedCategoriesString)
                            || selectedCategories.Contains(p.Category.CategoryName))
                .Where(p => !p.IsArchived);

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
            ViewData["IsAdmin"] = isAdmin;
            return PartialView("_ProductRows", inventoryList);
        }catch(Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("ServerError", "Error");
        }
     
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        try
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("GetProducts", "Product");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Product product)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index", "Admin", new { area = "" });
            }

            return View(product);
        }
        catch (Exception ex)
        {
            
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("ServerError", "Error");

        }
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id,
        [Bind("ProductId,ProductName,ProductPrice,ProductDescription,CategoryId,ProductStock")]
        Product product)
    {
        if (id != product.ProductId)
        {
            return RedirectToAction("NotFoundPage","Error");
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
                    return RedirectToAction("NotFoundPage","Error");
                }
                else
                {
                    return RedirectToAction("ServerError","Error");
                }
            }
        }

        return RedirectToAction("Index", "Admin", new { area = "" });
    }

    public bool ProductsExist(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("GetProducts", "Product");
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int productid)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productid);
            if (product != null)
            {
                product.IsArchived = true;
                _context.SaveChanges();
                return RedirectToAction("Index", "Admin", new { area = "" });
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("Delete", "Product");
        }
    }

}