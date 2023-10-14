﻿using AutoMapper;
using AutoMapper.Internal.Mappers;
using FreeCourse.Services.Order.Application.DTOs;
using FreeCourse.Services.Order.Application.Mapping;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Services.Order.Application.Handlers;

public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<OrderDto>>>
{
    private readonly OrderDbContext _context;

    public GetOrdersByUserIdQueryHandler(OrderDbContext orderDbContext)
    {
        _context = orderDbContext;
    }

    public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders.Include(x => x.OrderItems).Where(x => x.BuyerId == request.UserId)
            .ToListAsync();

        if (!orders.Any()) return Response<List<OrderDto>>.Success(new List<OrderDto>(), 200);

        List<OrderDto> orderDto = new List<OrderDto>();
        try
        {
            orderDto = ObjectMapper.Mapper.Map<List<OrderDto>>(orders);
            return Response<List<OrderDto>>.Success(orderDto, 200);
        }
        catch (AutoMapperMappingException ex)
        {
            var excep = ex.Message;
        }
        
        return Response<List<OrderDto>>.Success(orderDto, 200);

    }
}