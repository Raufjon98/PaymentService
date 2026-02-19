using System.Transactions;
using FluentAssertions;
using MagicOnion.Client;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Interfaces;
using PaymentService.Contracts.Transaction.Enum;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class GetCustomerTransactionsTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public GetCustomerTransactionsTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task GetCustomerTransactions_ReturnsTransactions_WhenTransactionsExist()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var accauntResponse = await _accountService.CreateAccountAsync(userId);
        accauntResponse.CustomerId.Should().Be(userId);

        var topUpRequest = new TopUpRequest()
        {
            CustomerId = userId,
            Amount = 100,
            SourceId = "IntegrationTest"
        };

        var topUpResponse = await _accountService.TopUpBalanceAsync(topUpRequest);
        topUpResponse.Balance.Should().Be(100);
        
        //Act
        var response = await _accountService.GetCustomerTransactionsAsync(userId);
        
        //Assert
        response.Should().AllBeOfType<TransactionResult>();
        response.Should().Contain(t=>t.Amount == 100 && t.TransactionType == TransactionType.TopUp);
    }
}