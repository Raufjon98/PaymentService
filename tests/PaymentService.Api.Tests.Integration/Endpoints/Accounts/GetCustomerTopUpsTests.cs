using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Interfaces;
using PaymentService.Contracts.Transaction.Enum;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class GetCustomerTopUpsTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public GetCustomerTopUpsTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task GetCustomerTopUps_ReturnsTopUps_WhenTopUpsExists()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createAccountResponse  = await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);

        var topUpRequest = new TopUpRequest
        {
            CustomerId = userId,
            Amount = 10,
            SourceId = "IntegrationTests"
        };
        
        var topUpResponse = await _accountService.TopUpBalanceAsync(topUpRequest);
        topUpResponse.Balance.Should().Be(10);

        //Act
        var response = await _accountService.GetCustomerToUpsAsync(userId);

        //Assert
        response.Should().NotBeNull();
        response.Should().Contain(t=> t.TransactionType == TransactionType.TopUp);
    }

    [Fact]
    public async Task GetCustomerTopUps_ThrowException_WhenCustomerDoesNotExist()
    {
        //Arrange
        var userId = Guid.NewGuid();
        
        //Act
        var response = await _accountService.GetCustomerToUpsAsync(userId);
        
        //Assert
        response.Should().BeEmpty();
    }
}