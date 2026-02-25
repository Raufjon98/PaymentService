using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Events;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Api.Application.Account.Commands.WithdrawBalance;

public record WithdrawBalanceCommand(WithdrawRequest WithdrawRequest) : IRequest<BalanceOperationResponse>;

public class WithdrawBalanceCommandHandler : IRequestHandler<WithdrawBalanceCommand, BalanceOperationResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public WithdrawBalanceCommandHandler(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<BalanceOperationResponse> Handle(WithdrawBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.CustomerId == request.WithdrawRequest.CustomerId, cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.WithdrawRequest.CustomerId.ToString());
        }

        if (account.Balance < request.WithdrawRequest.Amount)
        {
            throw new Exception("Insufficient balance");
        }

        account.Balance -= request.WithdrawRequest.Amount;
        _context.Accounts.Update(account);

        var transaction = new Domain.Entities.TransactionEntity
        {
            AccountId = account.Id,
            Amount = request.WithdrawRequest.Amount,
            TransactionType = TransactionType.Expense,
            Status = TransationStatus.Completed,
            SourceId = request.WithdrawRequest.SourceId,
        };

        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var publishAccountChangedEventTask = _publishEndpoint.Publish(
            new AccountUpdatedEvent
            {
                Id = account.Id,
                Balance = account.Balance,
                UpdatedOnUtc = DateTime.UtcNow
            },
            cancellationToken);
        
        var publishWithdrawalEventTask = _publishEndpoint.Publish(
            new WithdrawBalanceEvent
            {
                SourceId = request.WithdrawRequest.SourceId,
                CustomerId = account.CustomerId,
                Amount = transaction.Amount,
                TransactionStatus = transaction.Status,
            },
            cancellationToken);
        
        await Task.WhenAll(publishAccountChangedEventTask, publishWithdrawalEventTask);

        return new BalanceOperationResponse()
        {
            Balance = account.Balance,
            Message = $"Successfully Withdrawn: {request.WithdrawRequest.Amount}"
        };
    }
}