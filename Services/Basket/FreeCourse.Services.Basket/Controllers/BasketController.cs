﻿using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Basket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : CustomControllerBase
{
    private readonly IBasketService _basketService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public BasketController(IBasketService basketService, ISharedIdentityService sharedIdentityService)
    {
        _basketService = basketService;
        _sharedIdentityService = sharedIdentityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBasket()
    {
        var baskets = await _basketService.GetBasket(_sharedIdentityService.GetUserId);
        return CreateActionResultInstance(baskets);
    }

    [HttpPost]
    public async Task<IActionResult> SaveOrUpdateBasket(BasketDto request)
    {
        request.UserId = _sharedIdentityService.GetUserId;
        var response = await _basketService.SaveOrUpdate(request);
        return CreateActionResultInstance(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBasket()
    {
        return CreateActionResultInstance(await _basketService.Delete(_sharedIdentityService.GetUserId));
    }
    
}