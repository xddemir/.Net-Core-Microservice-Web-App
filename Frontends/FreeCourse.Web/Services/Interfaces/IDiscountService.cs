using FreeCourse.Web.Models.DiscountDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface IDiscountService
{
    Task<DiscountViewModel> GetDiscount(string discountCode);
}