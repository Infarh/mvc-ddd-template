using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SolutionTemplate.MVC.ViewModels;

namespace SolutionTemplate.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _Logger;

    public HomeController(ILogger<HomeController> logger) => _Logger = logger;

    public IActionResult Index() => View();

    [Route("~/Status/{Code}")]
    public IActionResult Status(string Code) => Content($"Status - {Code}");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}