using MagicOnion;
using MagicOnion.Server;
using MediatR;
using PaymentService.Api.Application.Account.Commands;
using PaymentService.Api.Application.Account.Queries;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Interfaces;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.MagicOnion.Services;

public class AccountService : ServiceBase<IAccountService>, IAccountService
{
    private readonly IMediator _mediator;

    public AccountService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async UnaryResult<AccountResponse> CreateAccountAsync(Guid customerId)
    {
       var command = new CreateAccountCommand(customerId);
       var result = await _mediator.Send(command);
       return result;
    }

    public async UnaryResult<BalanceOperationResponse> TopUpBalanceAsync(TopUpRequest topUpRequest)
    {
        var command = new TopUpBalanceCommand(topUpRequest);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<AccountResponse> UpdateAccountStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest)
    {
        var command = new UpdateAccountStatusCommand(updateAccountStatusRequest);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<BalanceOperationResponse> WithdrawBalanceAsync(WithdrawRequest withdrawRequest)
    {
        var command = new WithdrawBalanceCommand(withdrawRequest);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<BalanceOperationResponse> GetCustomerBalanceAsync(Guid customerId)
    {
        var query = new GetCustomerBalanceQuery(customerId);
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<List<TransactionResult>> GetCustomerToUpsAsync(Guid customerId)
    {
        var query = new GetCustomerTopUpsQuery(customerId);
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<List<TransactionResult>> GetCustomerWithdrasAsync(Guid customerId)
    {
        var query = new GetCustomerWithdrawsQuery(customerId);
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<List<TransactionResult>> GetCustomerTransactionsAsync(Guid customerId)
    {
        var query = new GetCustomerTransactionsQuery(customerId);
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<bool> DeleteAccountAsync(Guid customerId)
    {
        var command = new DeleteAccountCommand(customerId);
        var result = await _mediator.Send(command);
        return result;
    }
}