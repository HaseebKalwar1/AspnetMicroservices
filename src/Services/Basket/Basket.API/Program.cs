using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Discount.Grpc.Protos;
using MassTransit;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(StartupBase));

// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
//MassTransit-RabbitMQ Configuration 
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg0) =>
    {
        cfg0.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    }
    );
});
//builder.Services.AddMassTransitHostedService();

// Grpc Configuration
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
            (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
