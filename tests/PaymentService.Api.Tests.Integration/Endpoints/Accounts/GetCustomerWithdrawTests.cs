using FluentAssertions;
using MagicOnion.Client;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Interfaces;
using PaymentService.Contracts.Transaction.Enum;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class GetCustomerWithdrawTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public GetCustomerWithdrawTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task GetCustomerWithdraws_ReturnsWithdraws_WhenWithdrawExists()
    {
        //Arrange 
        var userId = Guid.NewGuid();
        
        var accountResponse = await _accountService.CreateAccountAsync(userId);
        accountResponse.CustomerId.Should().Be(userId);

        var topupRequest = new TopUpRequest
        {
            CustomerId = userId,
            Amount = 100,
            SourceId = "IntegrationTest"
        };
        
        var topUpResponse = await _accountService.TopUpBalanceAsync(topupRequest);
        topUpResponse.Balance.Should().Be(100);

        var withdrawRequest = new WithdrawRequest
        {
            CustomerId = userId,
            Amount = 10,
            SourceId = "IntegrationTests"
        };
        var withdrawResponse = await _accountService.WithdrawBalanceAsync(withdrawRequest);
        withdrawResponse.Balance.Should().Be(90);
        
        //Act
        var response = await _accountService.GetCustomerWithdrawsAsync(userId);
        
        //Assert
        response.Should().NotBeNull();
        response.Should().AllBeOfType<TransactionResult>();
        response.Should().Contain(w=>w.Amount == 10 && w.TransactionType == TransactionType.Expense);
    }

    [Fact]
    public async Task GetCustomerWithdraws_ReturnsEmpty_WhenNoWithdrawExists()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var accountResponse = await _accountService.CreateAccountAsync(userId);
        accountResponse.CustomerId.Should().Be(userId);
        
        //Act
        var response = await _accountService.GetCustomerWithdrawsAsync(userId);
        
        //Assert
        response.Should().BeEmpty();
    }
}