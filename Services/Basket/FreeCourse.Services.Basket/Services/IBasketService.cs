using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.DTOs;

namespace FreeCourse.Services.Basket.Services;

public interface IBasketService
{
    Task<Response<BasketItemDto>> GetBasket(string userId);
    Task<Response<bool>> SaveOrUpdate(BasketDto basketDto);
    Task<Response<bool>> Delete(string userId);
}