using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryBackOffice.Models;

namespace LibraryBackOffice.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        if (feature != null)
        {
            ViewBag.ErrorMessage = feature.Error.Message;
            ViewBag.Path = feature.Path;
        }
        return View();
    }
}
