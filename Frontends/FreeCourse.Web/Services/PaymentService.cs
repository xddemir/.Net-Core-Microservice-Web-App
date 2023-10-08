using FreeCourse.Web.Models.FakePaymentDtos;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class PaymentService: IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput)
    {
        var response = await _httpClient.PostAsJsonAsync<PaymentInfoInput>("fakePayment", paymentInfoInput);
        return response.IsSuccessStatusCode;
    }
}