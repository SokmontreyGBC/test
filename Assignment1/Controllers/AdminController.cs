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

        return View(inventory);
    }

}