using FreeCourse.Web.Models.CheckoutInfoDtos;
using FreeCourse.Web.Models.OrderDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface IOrderService
{
    Task<OrderStatusViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);
    Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput);
    Task<List<OrderViewModel>> GetOrder();
}