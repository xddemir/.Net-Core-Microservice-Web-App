using FreeCourse.Services.PhotoStock.DTOs;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.PhotoStock.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotosController : CustomControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
    {
        if (photo != null && photo.Length > 0)
        {
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
    
            await photo.CopyToAsync(stream, cancellationToken);

            var returnPath = fileName;

            var photoDto = new PhotoDto()
            {
                Url = returnPath
            };

            return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
        }

        return CreateActionResultInstance(Response<PhotoDto>.Fail("Photo is empty", 400));

    }

    public IActionResult PhotoDelete(string photoUrl)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

        if (!System.IO.File.Exists(path))
            return CreateActionResultInstance(Response<NoContent>.Fail("File does not exist", 404));
            
        System.IO.File.Delete(path);

        return CreateActionResultInstance(Response<NoContent>.Success(204));

    }
}