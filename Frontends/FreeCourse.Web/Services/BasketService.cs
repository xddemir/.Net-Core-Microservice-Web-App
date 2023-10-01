using FreeCourse.Shared.DTOs;
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

    public async Task<bool> DeleteAsync()
    {
        var response = await _httpClient.DeleteAsync("Basket");
        return response.IsSuccessStatusCode;
    }

    public async Task<BasketViewModel> Get()
    {
        var response = await _httpClient.GetAsync("basket");
        
        if (!response.IsSuccessStatusCode) return null;
        
        var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();
        return basketViewModel.Data;
    }

    public async Task<bool> SaveOrUpdateAsync(BasketViewModel basketViewModel)
    {
        var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("basket", basketViewModel);
        return response.IsSuccessStatusCode;
    }

    public async Task AddBasketItemAsync(BasketItemViewModel basketViewModel)
    {
        var basket = await Get();
        if (basket == null)
        {
            basket = new BasketViewModel();
            basket.BasketItems.Add(basketViewModel);
;       }
        else
        {
            if (!basket.BasketItems.Any(x => x.CourseId == basketViewModel.CourseId))
            {
                basket.BasketItems.Add(basketViewModel);
            }    
        }

        await SaveOrUpdateAsync(basket);

    }

    public async Task<bool> RemoveBasketItemAsync(string courseId)
    {
        var basket = await Get();

        if (basket == null) return false;

        var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);

        if (deleteBasketItem == null) return false;
        
        var deleteResult = basket.BasketItems.Remove(deleteBasketItem);

        if (deleteResult == null) return false;

        if (!basket.BasketItems.Any()) basket.DiscountCode = null;

        return await SaveOrUpdateAsync(basket);
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