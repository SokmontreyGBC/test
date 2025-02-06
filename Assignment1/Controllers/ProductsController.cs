using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

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
    
}