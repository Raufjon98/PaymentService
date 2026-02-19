using Grpc.Net.Client;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PaymentService.Api.Domain.Interfaces;
using PaymentService.Api.Infrastructure.Data;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace PaymentService.Api.Tests.Integration;

public class PaymentServiceApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("PaymentService")
            .WithUsername("Administrator")
            .WithPassword("Administrator@1234!")
            .WithCleanUp(true)
            .Build();

    private readonly RabbitMqContainer _rabbitMqContainer =
        new RabbitMqBuilder("rabbitmq:management")
            .WithCleanUp(true)
            .Build();

    private readonly RedisContainer _redisContainer =
        new RedisBuilder("redis")
            .WithCleanUp(true)
            .Build();

    private GrpcChannel? _channel;

    public GrpcChannel CreateGrpcChannel()
    {
        if (_channel == null)
        {
            var httpClient = CreateClient();
            _channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
        }

        return _channel;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.UseEnvironment("IntegrationTest");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });

            services.RemoveAll<IBus>();
            services.RemoveAll<IBusControl>();
            services.RemoveAll<IPublishEndpoint>();
            services.RemoveAll<ISendEndpointProvider>();
            var massTransitHealthCheck = services.FirstOrDefault(d =>
                d.ImplementationType?.FullName ==
                "MassTransit.AspNetCoreIntegration.HealthChecks.BusHealthCheck");
            if (massTransitHealthCheck != null)
                services.Remove(massTransitHealthCheck);

            services.AddMassTransitTestHarness(x =>
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(_rabbitMqContainer.GetConnectionString());
                    cfg.ConfigureEndpoints(ctx);
                });
            });

            services.RemoveAll<IDistributedCache>();
            services.RemoveAll<IDatabase>();
            services.RemoveAll<IConnectionMultiplexer>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _redisContainer.StartAsync();

        var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}