using System.Text.Json;
using Assignment1.Areas.Data;
using Assignment1.Areas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Areas.Client.Controllers;

/*
 *  This is the client inventory controller
 */
//[Route ("[controller]/[action]")]
public class ClientController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string selectedCategoryString = "")
    {
        var products = _context.Products
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
        ViewData["SelectedCategoryString"] = selectedCategoryString;

        return View(products);
    }

}
