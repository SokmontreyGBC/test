using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class CategoriesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}