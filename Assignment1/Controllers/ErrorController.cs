using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

//[Route ("[controller]/[action]")]
public class ErrorController : Controller
{
    [Route("Error/404")]
    public IActionResult NotFoundPage()
    {
        return View("NotFoundPage");
    }

    [Route("Error/500")]
    public IActionResult ServerError()
    {
        return View("InternalServerError");
    }

    [Route("Error/{statusCode}")]
    public IActionResult GlobalError(int statusCode)
    {
        if (statusCode == 404)
            return RedirectToAction("NotFoundPage");
        if (statusCode == 500)
            return RedirectToAction("ServerError");

        return View("GlobalError"); 
    }
}