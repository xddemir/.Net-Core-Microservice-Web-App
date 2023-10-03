using FreeCourse.Web.Models.CatalogDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface ICatalogService
{
    Task<List<CourseViewModel>> GetAllCoursesAsync();
    
    Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId);
    
    Task<CourseViewModel> GetByCourseIdAsync(string courseId);
    
    Task<List<CategoryViewModel>> GetAllCategoryAsync();

    Task<bool> CreateCourseAsync(CourseCreateInput request);

    Task<bool> UpdateCourseAsync(CourseUpdateInput request);

    Task<bool> DeleteCourseAsync(string courseId);

}