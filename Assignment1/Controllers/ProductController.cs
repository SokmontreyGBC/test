using System.Linq.Expressions;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

public class ProductController: Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
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
    }

}