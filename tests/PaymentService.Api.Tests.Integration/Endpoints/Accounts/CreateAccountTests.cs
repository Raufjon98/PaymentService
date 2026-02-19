using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;
using PaymentService.Api.Domain.Entities;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class CreateAccountTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public CreateAccountTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task CreateAccount_ReturnsAccountResponse_WhenDataIsValid()
    {
        //Arrange 
        var userId = Guid.NewGuid();
        
        //Act
        var response = await _accountService.CreateAccountAsync(userId);
        
        //Assert 
        response.Should().NotBeNull();
        response.Should().BeOfType<AccountResponse>();
        response.CustomerId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateAccount_ThrowsException_WhenAccaountAlredyExists()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createdAccount = await _accountService.CreateAccountAsync(userId);
        
        //Act
        Func<Task> act = async () =>  await _accountService.CreateAccountAsync(userId);
       
        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(AccountEntity)} with key {userId} exists!");
    }
}