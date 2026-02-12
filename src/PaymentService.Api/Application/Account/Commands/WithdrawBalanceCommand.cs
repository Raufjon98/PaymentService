using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Events;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Api.Application.Account.Commands;

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
            .Where(a => a.CustomerId == request.WithdrawRequest.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);

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

        await _publishEndpoint.Publish(
            new BalanceDecreaseEvent()
            {
                Id = account.Id,
                TransactionId = transaction.Id,
                Amount = request.WithdrawRequest.Amount,
                CurrentBalance = account.Balance,
                DecreasedOnUtc = DateTime.UtcNow
            },
            cancellationToken);

        return new BalanceOperationResponse()
        {
            Balance = account.Balance,
            Message = $"Successfully Withdrawn: {request.WithdrawRequest.Amount}"
        };
    }
}