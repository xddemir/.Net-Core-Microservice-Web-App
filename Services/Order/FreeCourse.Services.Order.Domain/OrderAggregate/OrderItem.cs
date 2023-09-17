using System.Globalization;
using FreeCourse.Services.Order.Domain.Core;

namespace FreeCourse.Services.Order.Domain.OrderAggregate;

public class OrderItem : Entity
{
    public string ProductId { get; private set; }
    public string ProductName { get; set; }
    public string PictureUrl { get; set; }
    public decimal Price { get; set; }

    public OrderItem(string productId, string productName, string pictureUrl, decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
    }

    public void UpdateOrderItem(string productName, string pictureUrl, decimal price)
    {
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
    }
}