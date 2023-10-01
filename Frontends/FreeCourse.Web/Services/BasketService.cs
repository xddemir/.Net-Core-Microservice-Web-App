using FreeCourse.Web.Models.BasketDtos;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;

    public BasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<bool> DeleteAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BasketViewModel> Get()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveOrUpdateAsync(BasketViewModel basketViewModel)
    {
        throw new NotImplementedException();
    }

    public Task AddBasketItemAsync(BasketItemViewModel basketViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveBasketItemAsync(string courseId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ApplyDiscountAsync(string discountCode)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CancelApplyDiscoutAsnyc()
    {
        throw new NotImplementedException();
    }
}