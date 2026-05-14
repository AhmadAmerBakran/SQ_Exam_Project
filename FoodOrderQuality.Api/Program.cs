using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;
using FoodOrderQuality.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DeliveryFeeCalculator>();
builder.Services.AddSingleton<IDiscountRepository, InMemoryDiscountRepository>();
builder.Services.AddSingleton<IOpeningHoursPolicy, WeeklyOpeningHoursPolicy>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<OrderPricingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/orders/quote", (OrderRequest request, OrderPricingService pricingService) =>
{
    var result = pricingService.Calculate(request);

    return result.Accepted
        ? Results.Ok(result)
        : Results.BadRequest(result);
});

app.Run();