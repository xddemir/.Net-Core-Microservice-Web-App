using FreeCourse.Web.Models.FakePaymentDtos;

namespace FreeCourse.Web.Services.Interfaces;

public interface IPaymentService
{
    Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput);

}