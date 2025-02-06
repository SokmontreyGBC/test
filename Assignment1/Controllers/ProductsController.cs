using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}