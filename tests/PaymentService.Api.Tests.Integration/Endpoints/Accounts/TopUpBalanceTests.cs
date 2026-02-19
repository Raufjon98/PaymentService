using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;
using PaymentService.Api.Domain.Entities;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class TopUpBalanceTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;
    
    public TopUpBalanceTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task TopUpBalance_ReturnsBalanseResponse_WhenTopUpSucceeded()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createAccountResponse = await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);

        var topUpRequest = new TopUpRequest
        {
            CustomerId = userId,
            SourceId = "integrationTest",
            Amount = 10,
        };
        
        //Act
        var response = await _accountService.TopUpBalanceAsync(topUpRequest);

        //Assert
        response.Balance.Should().Be(10);
    }

    [Fact]
    public async Task TopUpBalance_Throws_WhenAccountNotFound()
    {
        //Arrange
        var userId = Guid.NewGuid();
        
        var topUpRequest = new TopUpRequest
        {
            CustomerId = userId,
            SourceId = "integrationTest",
            Amount = 10,
        };
        
        //Act
        Func<Task> act = async () => await _accountService.TopUpBalanceAsync(topUpRequest);
        
        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(AccountEntity)} with key {userId} not found!");
    }
}