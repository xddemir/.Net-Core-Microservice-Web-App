using FreeCourse.Services.Order.Domain.OrderAggregate;

namespace FreeCourse.Services.Order.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedDate { get;  set; }
    public AddressDto Address { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}