using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : CustomControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISharedIdentityService _sharedIdentityService;

    public OrdersController(IMediator mediator, ISharedIdentityService identityService)
    {
        _mediator = mediator;
        _sharedIdentityService = identityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var response = await _mediator.Send(new GetOrdersByUserIdQuery{UserId = _sharedIdentityService.GetUserId});
        return CreateActionResultInstance(response);
    }

    [HttpPost]
    public async Task<IActionResult> SaveOrder(CreateOrderCommand request)
    {
        var createOrder = await _mediator.Send(request);
        return CreateActionResultInstance(createOrder);
    }
    
}