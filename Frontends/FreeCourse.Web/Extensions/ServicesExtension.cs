using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Extensions;

public static class ServicesExtension
{
    public static void AddHttpClientServices(this IServiceCollection Services, IConfiguration Configuration)
    {
        var serviceApiSettings = Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();
    
        Services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();

        Services.AddHttpClient<ICatalogService, CatalogService>(opt =>
        {
            opt.BaseAddress = new Uri(serviceApiSettings.GatewayBaseUri + "/" + serviceApiSettings.Catalog.Path);
        }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

        Services.AddHttpClient<IPhotoService, PhotoStockService>(opt =>
        {
            opt.BaseAddress = new Uri(serviceApiSettings.GatewayBaseUri + "/" + serviceApiSettings.PhotoStock.Path);
        }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

        Services.AddHttpClient<IUserService, UserService>(opt =>
        {
            opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        Services.AddHttpClient<IBasketService, BasketService>(opt =>
        {
            opt.BaseAddress = new Uri(serviceApiSettings.GatewayBaseUri + "/" + serviceApiSettings.Basket.Path);
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        Services.AddHttpClient<IIdentityService, IdentityService>();
    }
}