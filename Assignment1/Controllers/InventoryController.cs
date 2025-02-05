using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class InventoryController : Controller
{
    
    
    public IActionResult Index()
    {
        return View();
    }
    
    
}