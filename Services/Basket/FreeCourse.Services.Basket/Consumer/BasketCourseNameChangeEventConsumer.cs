using System.Text.Json;
using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;

namespace FreeCourse.Services.Basket.Consumer;

public class BasketCourseNameChangeEventConsumer: IConsumer<CourseNameChangedEvent>
{
    private readonly RedisService _redisService;

    public BasketCourseNameChangeEventConsumer(RedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
    {
        var response = await _redisService.GetDb().StringGetAsync(context.Message.UserId);;
        var basket = JsonSerializer.Deserialize<BasketDto>(response);
        
        basket.basketItems.ForEach(x =>
        {
            if (x.CourseId == context.Message.CourseId)
                x.CourseName = context.Message.UpdatedName;
        });

        await _redisService.GetDb().StringSetAsync(basket.UserId, JsonSerializer.Serialize(basket));
    }
}