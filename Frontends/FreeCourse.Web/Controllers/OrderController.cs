using FreeCourse.Web.Models.CheckoutInfoDtos;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

public class OrderController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IOrderService _orderService;

    public OrderController(IBasketService basketService, IOrderService orderService)
    {
        _basketService = basketService;
        _orderService = orderService;
    }

    // GET
    public async Task<IActionResult> Checkout()
    {
        var basket = await _basketService.Get();

        ViewBag.basket = basket;
        
        return View(new CheckoutInfoInput());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
    {
        // Sync
        // var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
        
        // Async
        var orderStatusSuspend = await _orderService.SuspendOrder(checkoutInfoInput);
        
        if (!orderStatusSuspend.IsSuccessful)
        {
            var basket = await _basketService.Get();
            ViewBag.basket = basket;
            ViewBag.error = orderStatusSuspend.Error;
            return View();
        }
        
        // sync
        // return RedirectToAction(nameof(SuccessfulCheckout), new {orderId = orderStatusSuspend.OrderId});
        
        return RedirectToAction(nameof(SuccessfulCheckout), new {orderId = new Random().Next(1, 1000)});
    }

    public IActionResult SuccessfulCheckout(int orderId)
    {
        ViewBag.orderId = orderId;
        return View();
    }

    public async Task<IActionResult> CheckoutHistory()
    {
        return View(await _orderService.GetOrder());
    }
}