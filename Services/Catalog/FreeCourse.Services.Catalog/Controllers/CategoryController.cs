using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Catalog.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController: CustomControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllByAsync()
    {
        var result = await _categoryService.GetAllAsync();
        return CreateActionResultInstance(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return CreateActionResultInstance(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDTO request)
    {
        var result = await _categoryService.CreateAsync(request);
        return CreateActionResultInstance(result);
    }
    
    
    
}