using AutoMapper;
using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Messages;
using MassTransit;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services;

public class CourseService : ICourseService
{
    private readonly IMongoCollection<Course> _courseCollection;
    private readonly IMongoCollection<Category> _categoryCollection;

    private readonly IPublishEndpoint _publishEndpoint;
    
    private readonly IMapper _mapper;

    public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, IPublishEndpoint publishEndpoint)
    {
        var client = new MongoClient(databaseSettings.ConnectionString);
        var database = client.GetDatabase(databaseSettings.DatabaseName);
        
        _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
        _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Shared.DTOs.Response<List<CourseDTO>>> GetAllAsync()
    {
        var courses = await _courseCollection.Find(course => true).ToListAsync();

        if (courses.Any())
        {
            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find<Category>(x=> x.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = new List<Course>();
        }

        return Shared.DTOs.Response<List<CourseDTO>>.Success(_mapper.Map<List<CourseDTO>>(courses), 200);
    }

    public async Task<Shared.DTOs.Response<CourseDTO>> GeyByIdAsync(string id)
    {
        var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

        if (course == null)
            return Shared.DTOs.Response<CourseDTO>.Fail("Couldn't find the error", 404);

        course.Category = await _categoryCollection.Find<Category>(c => c.Id == course.CategoryId).FirstAsync();

        return Shared.DTOs.Response<CourseDTO>.Success(_mapper.Map<CourseDTO>(course), 200);

    }
    
    public async Task<Shared.DTOs.Response<List<CourseDTO>>> GeyAllByUserId(string userId)
    {
        var courses = await _courseCollection.Find(course => course.UserId == userId).ToListAsync();

        if (courses.Any())
        {
            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find(category => category.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = new List<Course>();
        }

        return Shared.DTOs.Response<List<CourseDTO>>.Success(_mapper.Map<List<CourseDTO>>(courses), 200);
    }

    public async Task<Shared.DTOs.Response<CourseDTO>> CreateAsync(CourseCreateDTO request)
    {
        var newCourse = _mapper.Map<Course>(request);
        
        newCourse.CreatedTime = DateTime.Now;

        await _courseCollection.InsertOneAsync(newCourse);

        return Shared.DTOs.Response<CourseDTO>.Success(_mapper.Map<CourseDTO>(newCourse), 201);

    }

    public async Task<Shared.DTOs.Response<NoContent>> UpdateAsync(CourseUpdateDTO request)
    {
        var updatedCourse = _mapper.Map<Course>(request);

        var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == updatedCourse.Id, updatedCourse);

        if (result == null)
            return Shared.DTOs.Response<NoContent>.Fail("Content not found", 404);

        await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent()
        {
            UserId = updatedCourse.UserId,
            CourseId = updatedCourse.Id,
            UpdatedName = request.Name
        });

        return Shared.DTOs.Response<NoContent>.Success(204);
    }

    public async Task<Shared.DTOs.Response<NoContent>> DeleteAsync(string id)
    {
        var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);

        if (result.DeletedCount > 0)
            return Shared.DTOs.Response<NoContent>.Success(204);

        return Shared.DTOs.Response<NoContent>.Fail("Course not found", 404);

    }
}