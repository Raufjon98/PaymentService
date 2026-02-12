using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;
using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Api.Application.Account.Commands;

public record TopUpBalanceCommand(TopUpRequest TopUpRequest) : IRequest<BalanceOperationResponse>;

public class TopUpBalanceCommandHandler : IRequestHandler<TopUpBalanceCommand, BalanceOperationResponse>
{
    private readonly ApplicationDbContext _context;

    public TopUpBalanceCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BalanceOperationResponse> Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a=> a.CustomerId == request.TopUpRequest.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync( cancellationToken);

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

        return new BalanceOperationResponse()
        {
            Balance = account.Balance,
            Message = $"Successfully TopUp: {request.TopUpRequest.Amount}"
        };
    }
}