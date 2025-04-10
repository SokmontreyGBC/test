using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // Write list of categories to variable
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Category category)
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