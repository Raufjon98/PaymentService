using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class WithdrawBalanceTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public WithdrawBalanceTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task WithdrawBalance_ReturnsBalanceResponse_WhenSucceeded()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createAccountResponse = await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);

        var topUpRequest = new TopUpRequest
        {
            SourceId = "IntegrationTests",
            CustomerId = userId,
            Amount = 100,
        };
        var topUpResponse = await _accountService.TopUpBalanceAsync(topUpRequest);
        topUpResponse.Balance.Should().Be(100);

        var withdrawREquest = new WithdrawRequest
        {
            SourceId = "IntegrationTests",
            CustomerId = userId,
            Amount = 50,
        };

        //Act
        var response = await _accountService.WithdrawBalanceAsync(withdrawREquest);

        //Assert
        response.Balance.Should().Be(50);
    }

    [Fact]
    public async Task WithdrawBalance_ThrowException_WhenBalanceMoneyIsNotEnough()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createAccountResponse = await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);

        var topUpRequest = new TopUpRequest
        {
            SourceId = "IntegrationTests",
            CustomerId = userId,
            Amount = 25,
        };
        var topUpResponse = await _accountService.TopUpBalanceAsync(topUpRequest);
        topUpResponse.Balance.Should().Be(25);

        var withdrawREquest = new WithdrawRequest
        {
            SourceId = "IntegrationTests",
            CustomerId = userId,
            Amount = 50,
        };

        //Act
        Func<Task> act = async () => await _accountService.WithdrawBalanceAsync(withdrawREquest);

        //Assert
        await act.Should().ThrowAsync<RpcException>("Insufficient balance");
    }
}