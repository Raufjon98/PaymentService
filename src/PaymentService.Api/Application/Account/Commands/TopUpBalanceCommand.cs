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

public record TopUpBalanceCommand(TopUpRequest TopUpRequest) : IRequest<BalanceOperationResponse>;

public class TopUpBalanceCommandHandler : IRequestHandler<TopUpBalanceCommand, BalanceOperationResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TopUpBalanceCommandHandler(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<BalanceOperationResponse> Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a=> a.CustomerId == request.TopUpRequest.CustomerId, cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.TopUpRequest.CustomerId.ToString());
        }

        account.Balance += request.TopUpRequest.Amount;
        _context.Accounts.Update(account);
        var transaction = new Domain.Entities.TransactionEntity
        {
            AccountId = account.Id,
            Amount = request.TopUpRequest.Amount,
            TransactionType = TransactionType.TopUp,
            Status = TransationStatus.Completed,
            SourceId = request.TopUpRequest.SourceId,
        };
        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        await _publishEndpoint.Publish(
            new AccountUpdatedEvent
            {
               Id = account.Id,
               Balance = account.Balance,
               UpdatedOnUtc = DateTime.UtcNow
            },
            cancellationToken);

        return new BalanceOperationResponse()
        {
            Balance = account.Balance,
            Message = $"Successfully TopUp: {request.TopUpRequest.Amount}"
        };
    }
}