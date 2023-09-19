using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FakePaymentController: CustomControllerBase
{
    [HttpPost]
    public IActionResult ReceivePayment()
    {
        return CreateActionResultInstance(Response<NoContent>.Success(200));
    }
}