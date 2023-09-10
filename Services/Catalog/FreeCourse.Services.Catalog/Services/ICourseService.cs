using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Shared.DTOs;

namespace FreeCourse.Services.Catalog.Services;

public interface ICourseService
{
    Task<Response<List<CourseDTO>>> GetAllAsync();
    Task<Response<CourseDTO>> GeyByIdAsync(string id);
    Task<Response<List<CourseDTO>>> GeyAllByUserId(string userId);
    Task<Response<CourseDTO>> CreateAsync(CourseCreateDTO request);
    Task<Response<NoContent>> UpdateAsync(CourseUpdateDTO request);
    Task<Response<NoContent>> DeleteAsync(string id);
}