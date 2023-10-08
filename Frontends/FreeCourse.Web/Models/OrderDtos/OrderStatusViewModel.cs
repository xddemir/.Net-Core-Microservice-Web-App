namespace FreeCourse.Web.Models.OrderDtos;

public class OrderStatusViewModel
{
    public int OrderId { get; set; }
    public string Error { get; set; }
    public bool IsSuccessful { get; set; }
}