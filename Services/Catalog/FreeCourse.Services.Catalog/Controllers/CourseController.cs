using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Catalog.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController : CustomControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // course/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await _courseService.GeyByIdAsync(id);
        return CreateActionResultInstance(response);
    }
    
    // course/GetAllByUserId/5
    [Route("/api/[controller]/GetAllByUserId/{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var response = await _courseService.GeyAllByUserId(userId);
        return CreateActionResultInstance(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _courseService.GetAllAsync();
        return CreateActionResultInstance(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateDTO request)
    {
        var response = await _courseService.CreateAsync(request);
        return CreateActionResultInstance(response);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(CourseUpdateDTO request)
    {
        var response = await _courseService.UpdateAsync(request);
        return CreateActionResultInstance(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _courseService.DeleteAsync(id);
        return CreateActionResultInstance(response);
    }
    
}