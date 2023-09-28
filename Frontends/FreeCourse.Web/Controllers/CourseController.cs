using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.CatalogDtos;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreeCourse.Web.Controllers;

[Authorize]
public class CourseController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CourseController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
    {
        _catalogService = catalogService;
        _sharedIdentityService = sharedIdentityService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var courses = await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId);
        return View(courses);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.categoryList = new SelectList(categories, "Id", "Name");

        return View();
;    }

    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateInput request)
    {
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.categoryList = new SelectList(categories, "Id", "Name");

        request.UserId = _sharedIdentityService.GetUserId;

        if (!ModelState.IsValid) return View(); 

        await _catalogService.CreateCourseAsync(request);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(string id)
    {
        var course = await _catalogService.GetByCourseIdAsync(id);

        if (course == null) return RedirectToAction(nameof(Index));
        
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.categoryList = new SelectList(categories, "Id", "Name", course.Id);

        CourseUpdateInput courseUpdateInput = new()
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            Feature = course.Feature,
            CategoryId = course.CategoryId,
            UserId = course.UserId,
            Picture = course.Picture
        };

        return View(courseUpdateInput);
    }


    [HttpPost]
    public async Task<IActionResult> Update(CourseUpdateInput request)
    {
        var categories = await _catalogService.GetAllCategoryAsync();
        ViewBag.categoryList = new SelectList(categories, "Id", "Name", request.Id);

        if (!ModelState.IsValid) return View();

        await _catalogService.UpdateCourseAsync(request);

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Delete(string id)
    {
        await _catalogService.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }
}