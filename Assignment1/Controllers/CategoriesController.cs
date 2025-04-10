using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

//[Route ("[controller]/[action]")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ApplicationDbContext context, ILogger<CategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("NotFoundPage", "Error");
        }
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        try
        {
            // Write list of categories to variable
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("NotFoundPage", "Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Category category)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (CheckIfCategoryExists(category.CategoryName))
                {
                    TempData["ErrorMessage"] = "Category already exists";
                    return RedirectToAction("Create");

                }

                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("ServerError", "Error");
        }
    }

    public bool CheckIfCategoryExists(string categoryName)
    {
        var category = _context.Categories.ToList();
        foreach (var item in category)
        {
            if (item.CategoryName == categoryName)
            {
                return true;
            }
        }
        return false;
    }
    
}