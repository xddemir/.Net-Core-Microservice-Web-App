using FreeCourse.Web.Models.PhotoStockDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface IPhotoService
{
    Task<PhotoViewModel> UploadPhoto(IFormFile photo);
    Task<bool> DeletePhoto(string photoUrl);
    
}