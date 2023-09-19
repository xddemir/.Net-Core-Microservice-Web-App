﻿using AutoMapper;
using FreeCourse.Services.Order.Application.DTOs;
using FreeCourse.Services.Order.Domain.OrderAggregate;

namespace FreeCourse.Services.Order.Application.Mapping;

public class CustomMapping : Profile
{
    public CustomMapping()
    {
        CreateMap<Domain.OrderAggregate.Order, OrderDto>().ReverseMap();
        CreateMap<Domain.OrderAggregate.OrderItem, OrderItem>().ReverseMap();
        CreateMap<Domain.OrderAggregate.Address, AddressDto>().ReverseMap();
    }
}