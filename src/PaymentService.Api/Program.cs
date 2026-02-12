using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Api.MagicOnion.Services;
using PaymentService.Contracts.Interfaces;
using MediatR;
using PaymentService.Api.Infrastructure.Interceptors;

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
