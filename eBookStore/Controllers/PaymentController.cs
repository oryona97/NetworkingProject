using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;

namespace eBookStore.Controllers;

public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View("checkout");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
