using System.Text;
using System.Text.Json;
using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models.BasketDtos;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    private readonly IDiscountService _discountService;

    public BasketService(HttpClient httpClient, IDiscountService discountService)
    {
        _httpClient = httpClient;
        _discountService = discountService;
    }

    public async Task<bool> DeleteAsync()
    {
        var response = await _httpClient.DeleteAsync("basket");
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
        var content = JsonConvert.SerializeObject(basketViewModel);
        var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("basket", stringContent);
        return response.IsSuccessStatusCode;
    }

    public async Task AddBasketItemAsync(BasketItemViewModel basketItemViewModel)
    {
        var basket = await Get();

        if (basket != null)
        {
            if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
            {
                basket.BasketItems.Add(basketItemViewModel);
            }
        }
        else
        {
            basket = new BasketViewModel();

            basket.BasketItems.Add(basketItemViewModel);
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

    public async Task<bool> ApplyDiscountAsync(string discountCode)
    {
        await CancelApplyDiscoutAsnyc();
    
        var basket = await Get();

        if (basket == null) return false;
        
        var response = await _discountService.GetDiscount(discountCode);

        if (response == null) return false;

        basket.DiscountRate = response.Rate;
        basket.DiscountCode = response.Code;

        await SaveOrUpdateAsync(basket);

        return true;

    }

    public async Task<bool> CancelApplyDiscoutAsnyc()
    {
        var basket = await Get();
        if (basket == null && basket.DiscountCode == null) return false;

        basket.DiscountCode = null;
        basket.DiscountRate = null;

        await SaveOrUpdateAsync(basket);
        
        return true;

    }
    
}