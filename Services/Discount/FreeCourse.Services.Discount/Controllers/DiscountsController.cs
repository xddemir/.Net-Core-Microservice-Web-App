using FreeCourse.Services.Discount.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Discount.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountsController : CustomControllerBase
{
   private readonly IDiscountService _discountService;
   private readonly ISharedIdentityService _identityService;
   
   public DiscountsController(IDiscountService discountService, ISharedIdentityService identityService)
   {
      _discountService = discountService;
      _identityService = identityService;
   }

   [HttpGet]
   public async Task<IActionResult> GetAll()
   {
      return CreateActionResultInstance(await _discountService.GetAll());
   }
   
   [HttpGet("{id}")]
   public async Task<IActionResult> GetById(int Id)
   {
      return CreateActionResultInstance(await _discountService.GetById(Id));
   }
   
   [HttpGet]
   [Route("/api/[controller]/[action]/{code}")]
   public async Task<IActionResult> GetByCode(string code)
   {
      var userId = _identityService.GetUserId;
      var discount = await _discountService.GetByCodeAndUserId(code, userId);

      return CreateActionResultInstance(discount);
   }

   [HttpPost]
   public async Task<IActionResult> Save(Models.Discount request)
   {
      return CreateActionResultInstance(await _discountService.Save(request));
   }
   
   [HttpPut]
   public async Task<IActionResult> Update(Models.Discount request)
   {
      return CreateActionResultInstance(await _discountService.Update(request));
   }
   
   [HttpDelete("{id}")]
   public async Task<IActionResult> Delete(int id)
   {
      return CreateActionResultInstance(await _discountService.Delete(id));
   }
}