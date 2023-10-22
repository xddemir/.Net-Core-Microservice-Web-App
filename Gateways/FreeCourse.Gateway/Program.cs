using FreeCourse.Gateway.DelegateHandlers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot().AddDelegatingHandler<TokenExchangeDelegateHandler>();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"configuration.{hostingContext.HostingEnvironment.EnvironmentName.ToLower()}.json")
        .AddEnvironmentVariables();
});

builder.Services.AddHttpClient<TokenExchangeDelegateHandler>();

builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_gateway";
    options.RequireHttpsMetadata = false;
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.UseAuthentication();

app.Run();