using FluentAssertions;
using MagicOnion.Client;
using PaymentService.Api.Domain.Entities;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class GetCustomerBalanceTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public GetCustomerBalanceTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task GetBalance_ReturnsBalance_WhenAccountExists()
    {
        //Arrange 
        var userId = Guid.NewGuid();
        var createAccountResponse = await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);
        
        //Act
        var response = await _accountService.GetCustomerBalanceAsync(userId);
        
        //Assert
        response.Should().NotBeNull();
        response.Balance.Should().Be(0);
    }
    
    [Fact]
    public async Task GetBalance_ThrowException_WhenAccountDoesNotExists()
    {
        //Arrange 
        var userId = Guid.NewGuid();
        
        //Act
        Func<Task> act = async () => await _accountService.GetCustomerBalanceAsync(userId);
        
        //Assert
        await act.Should().ThrowAsync($"Entity {nameof(AccountEntity)} with key {userId} not found!");
    }
}