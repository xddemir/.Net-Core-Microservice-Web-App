using System.Diagnostics;
using FreeCourse.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

namespace FreeCourse.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ICatalogService _catalogService;

    public HomeController(ILogger<HomeController> logger, ICatalogService catalogService)
    {
        _logger = logger;
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _catalogService.GetAllCoursesAsync();
        return View(courses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Detail(string id)
    {
        return View(await _catalogService.GetByCourseIdAsync(id));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (errFeature != null && errFeature.Error is UnauthorizedException)
        {
            return RedirectToAction(nameof(AuthController.Logout), "Auth");
        }
    
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}