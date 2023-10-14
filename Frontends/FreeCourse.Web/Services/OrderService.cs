using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.CheckoutInfoDtos;
using FreeCourse.Web.Models.FakePaymentDtos;
using FreeCourse.Web.Models.OrderDtos;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class OrderService : IOrderService
{
    private readonly IPaymentService _paymentService;
    private readonly HttpClient _httpClient;
    private readonly IBasketService _basketService;
    private readonly ISharedIdentityService _identityService;

    public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, ISharedIdentityService identityService)
    {
        _paymentService = paymentService;
        _httpClient = httpClient;
        _basketService = basketService;
        _identityService = identityService;
    }

    public async Task<OrderStatusViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
    {
        var basket = await _basketService.Get();

        var paymentInfoInput = new PaymentInfoInput()
        {
            CardName = checkoutInfoInput.CardName,
            CardNumber = checkoutInfoInput.CardNumber,
            Expiration = checkoutInfoInput.Expiration,
            CVV = checkoutInfoInput.CVV,
            TotalPrice = basket.TotalPrice
        };

        var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);

        if (!responsePayment)
            return new OrderStatusViewModel() { Error = "Payment not received!" };

        var orderCreateInput = new OrderCreateInput()
        {
            BuyerId = _identityService.GetUserId,
            Address = new AddressCreateInput()
            {
                Province = checkoutInfoInput.Province, District = checkoutInfoInput.District,
                Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode
            },
        };
        
        basket.BasketItems.ForEach(x =>
        {
            var orderItem = new OrderItemCreateInput()
                { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };
            orderCreateInput.OrderItems.Add(orderItem);
        });

        var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);

        if (response.IsSuccessStatusCode)
            return new OrderStatusViewModel() { Error = "Order not created!" };
        
        var orderCreatedVıewModel =  await response.Content.ReadFromJsonAsync<Response<OrderStatusViewModel>>();
        orderCreatedVıewModel.Data.IsSuccessful = true;

        await _basketService.DeleteAsync();
        
        return orderCreatedVıewModel.Data;
    }

    public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
    {
        var orderCreateInput = new OrderCreateInput()
        {
            BuyerId = _identityService.GetUserId,
            Address = new AddressCreateInput()
            {
                Province = checkoutInfoInput.Province, District = checkoutInfoInput.District,
                Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode
            },
        };
        
        var basket = await _basketService.Get();

        basket.BasketItems.ForEach(x =>
        {
            var orderItem = new OrderItemCreateInput()
                { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };
            orderCreateInput.OrderItems.Add(orderItem);
        });
    

        var paymentInfoInput = new PaymentInfoInput()
        {
            CardName = checkoutInfoInput.CardName,
            CardNumber = checkoutInfoInput.CardNumber,
            Expiration = checkoutInfoInput.Expiration,
            CVV = checkoutInfoInput.CVV,
            TotalPrice = basket.TotalPrice,
            Order = orderCreateInput
        };
        
        var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);

        if (!responsePayment)
            return new OrderSuspendViewModel { Error = "Order not created", IsSuccessful = false };

        await _basketService.DeleteAsync();

        return new OrderSuspendViewModel() { IsSuccessful = true };

    }

    public async Task<List<OrderViewModel>> GetOrder()
    {
        var content = await _httpClient.GetAsync("orders");
        var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");
        return response.Data;
    }
    
    
}