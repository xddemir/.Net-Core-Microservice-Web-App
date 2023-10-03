using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.CatalogDtos;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly IPhotoService _photoService;
    private readonly PhotoHelper _photoHelper;

    public CatalogService(HttpClient httpClient, IPhotoService photoService, PhotoHelper photoHelper)
    {
        _httpClient = httpClient;
        _photoService = photoService;
        _photoHelper = photoHelper;
    }
    
    // localhost:5000/services/catalog/course
    public async Task<List<CourseViewModel>> GetAllCoursesAsync()
    {
        var responses = await _httpClient.GetAsync("course");
        
        if (!responses.IsSuccessStatusCode) return null;

        var result = await responses.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        result.Data.ForEach(x =>
        {
            x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
        });

        return result.Data;

    }
    
    // localhost:5000/services/catalog/category/GetAllByUserId/{userId}
    public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
    {
        var responses = await _httpClient.GetAsync($"course/GetAllByUserId/{userId}");
        
        if (!responses.IsSuccessStatusCode) return null;

        var result = await responses.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
        
        result.Data.ForEach(x =>
        {
            x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
        });
        
        return result.Data;
    }
    
    // localhost:5000/services/catalog/course/{courseId}
    public async Task<CourseViewModel> GetByCourseIdAsync(string courseId)
    {
        var responses = await _httpClient.GetAsync($"course/{courseId}");
        
        if (!responses.IsSuccessStatusCode) return null;

        var result = await responses.Content.ReadFromJsonAsync<Response<CourseViewModel>>();

        result.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(result.Data.Picture);

        return result.Data;
    }

    // localhost:5000/services/catalog/category
    public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
    {
        var responses = await _httpClient.GetAsync("category");
        
        if (!responses.IsSuccessStatusCode) return null;

        var result = await responses.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();

        return result.Data;
    }

    // localhost:5000/services/catalog/category
    public async Task<bool> CreateCourseAsync(CourseCreateInput request)
    {
        var resultPhotoService = await _photoService.UploadPhoto(request.PhotoFormFile);
        
        if (resultPhotoService != null) request.Picture = resultPhotoService.Url;
            
        var response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("course", request);
        return response.IsSuccessStatusCode;
    }
    
    // localhost:5000/services/catalog/category
    public async Task<bool> UpdateCourseAsync(CourseUpdateInput request)
    {
        var resultPhotoService = await _photoService.UploadPhoto(request.PhotoFormFile);

        if (resultPhotoService != null)
        {
            await _photoService.DeletePhoto(request.Picture);
            request.Picture = resultPhotoService.Url;
        }
    
        var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("course", request);
        return response.IsSuccessStatusCode;

    }

    // localhost:5000/services/catalog/category/{courseId}
    public async Task<bool> DeleteCourseAsync(string courseId)
    {
        var response = await _httpClient.DeleteAsync($"course/{courseId}");
        return response.IsSuccessStatusCode;
    }
}