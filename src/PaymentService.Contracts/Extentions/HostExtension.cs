using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Contracts.Extentions;

public static class HostExtension
{
    public static IServiceCollection AddPaymentServiceContracts(this IServiceCollection services)
    {
        var paymentServiceUrl = "https://localhost:5011";
       
        services.AddSingleton<IAccountService>(_ => 
            MagicOnionClient.Create<IAccountService>(GrpcChannel.ForAddress(paymentServiceUrl)));

        return services;
    }
}