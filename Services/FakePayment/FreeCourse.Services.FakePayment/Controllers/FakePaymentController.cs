using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FakePaymentController: CustomControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public FakePaymentController(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost]
    public async Task<IActionResult> ReceivePayment(PaymentDto request)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:order-created-service"));

        var createOrderMessageCommand = new CreateOrderMessageCommand();

        createOrderMessageCommand.BuyerId = request.Order.BuyerId;
        createOrderMessageCommand.Province = request.Order.Address.Province;
        createOrderMessageCommand.District = request.Order.Address.District;
        createOrderMessageCommand.Street = request.Order.Address.Street;
        createOrderMessageCommand.Line = request.Order.Address.Line;
        createOrderMessageCommand.ZipCode = request.Order.Address.ZipCode;
        
        request.Order.OrderItems.ForEach(x =>
        {
             createOrderMessageCommand.OrderItems.Add(new OrderItem()
             {
                 PictureUrl = x.PictureUrl,
                 Price = x.Price,
                 ProductName = x.ProductName,
                 ProductId = x.ProductId
             });
        });

        await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);
    
        return CreateActionResultInstance(Shared.DTOs.Response<NoContent>.Success(200));
    }
}