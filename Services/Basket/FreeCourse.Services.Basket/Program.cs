using System.IdentityModel.Tokens.Jwt;
using System.Net;
using FreeCourse.Services.Basket.Consumer;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var requireAuthPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter(requireAuthPolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_basket";
    options.RequireHttpsMetadata = false;
});

builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>();
    var redis = new RedisService(redisSettings);
    redis.Connect();

    return redis;
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BasketCourseNameChangeEventConsumer>();

    x.UsingRabbitMq((contex, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQURL"], "/", configurator =>
        {
            configurator.Username("guest");
            configurator.Password("guest");            
        } );
        
        cfg.ReceiveEndpoint("course-name-change-event-basket-service", e =>
        {
            e.ConfigureConsumer<BasketCourseNameChangeEventConsumer>(contex);
        });
    });
});

builder.Services.AddMassTransitHostedService();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

