using FreeCourse.Web.Models.BasketDtos;
using FreeCourse.Web.Models.CatalogDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface IBasketService
{
    Task<bool> DeleteAsync();
    Task<BasketViewModel> Get();
    Task<bool> SaveOrUpdateAsync(BasketViewModel basketViewModel);
    Task AddBasketItemAsync(BasketItemViewModel basketViewModel);
    Task<bool> RemoveBasketItemAsync(string courseId);
    Task<bool> ApplyDiscountAsync(string discountCode);
    Task<bool> CancelApplyDiscoutAsnyc();
}