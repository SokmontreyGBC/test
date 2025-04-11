using System.Globalization;
using System.Linq.Expressions;
using Assignment1.Areas.Data;
using Assignment1.Areas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Areas.Admin.Controllers;

/*
 *  This is the employee inventory controller
 *  It is responsible for non-client facing actions
 */

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
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
        catch (Exception ex)
        {
            var user = User.Identity?.Name ?? "Anonymous";
            _logger.LogError(ex, ex.Message + "\n User:" + user );
            return RedirectToAction("ServerError", "Error");
        }
    }

}
