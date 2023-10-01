using FreeCourse.Web.Models.BasketDtos;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

public class BasketController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly IBasketService _basketService;

    public BasketController(ICatalogService catalogService, IBasketService basketService)
    {
        _catalogService = catalogService;
        _basketService = basketService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View(await _basketService.Get());
    }

    public async Task<IActionResult> AddBasketItem(string courseId)
    {
        var course = await _catalogService.GetByCourseIdAsync(courseId);
        var basketItem = new BasketItemViewModel()
        {
            CourseId = courseId,
            CourseName = course.Name,
            Price = course.Price,
            Quantity = 1
        };

        await _basketService.AddBasketItemAsync(basketItem);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveBasketItem(string courseId)
    {
        await _basketService.RemoveBasketItemAsync(courseId);
        return RedirectToAction(nameof(Index));
    }
}