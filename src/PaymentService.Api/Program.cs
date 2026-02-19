using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Api.MagicOnion.Services;
using PaymentService.Contracts.Interfaces;
using MediatR;
using PaymentService.Api.Infrastructure.Interceptors;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5011, listenOptions =>
    {
        listenOptions.UseHttps();              
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var rabbitConnectionString = builder.Configuration["MessageBroker:Host"];

if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.Services.AddMassTransit(configuration =>
    {
        configuration.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(rabbitConnectionString);
            cfg.ExchangeType = ExchangeType.Fanout;
            cfg.ConfigureEndpoints(ctx);
        });
    });
}

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});
builder.Services.AddMagicOnion();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapMagicOnionService();
app.MapGet("/", () => Console.WriteLine("Payment Service"));
app.Run();
