using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;
using PaymentService.Api.Domain.Entities;
using PaymentService.Contracts.Interfaces;

namespace PaymentService.Api.Tests.Integration.Endpoints.Accounts;

public class DeleteAccountTests : IClassFixture<PaymentServiceApiFactory>
{
    private readonly PaymentServiceApiFactory _factory;
    private readonly IAccountService _accountService;

    public DeleteAccountTests(PaymentServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _accountService = MagicOnionClient.Create<IAccountService>(channel);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsTrue_WhenAccountExists()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var createAccountResponse =  await _accountService.CreateAccountAsync(userId);
        createAccountResponse.CustomerId.Should().Be(userId);
        
        //Act
        var response = await _accountService.DeleteAccountAsync(userId);
        
        //Assert
        response.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAccount_ThrowsException_WhenAccountDoesNotExist()
    {
        //Arrange 
        var userId = Guid.NewGuid();
        
        //Act
        Func<Task> act = async() => await _accountService.DeleteAccountAsync(userId);
        
        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(AccountEntity)} with key {userId} not found!");
    }
}