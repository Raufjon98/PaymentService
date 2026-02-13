using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Application.Account.Queries;

public record GetCustomerTransactionsQuery(Guid CustomerId) : IRequest<List<TransactionResult>>;

public class GetCustomerTransactionsQueryHandler : IRequestHandler<GetCustomerTransactionsQuery, List<TransactionResult>>
{
    private readonly ApplicationDbContext _context;

    public GetCustomerTransactionsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TransactionResult>> Handle(GetCustomerTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .Include(t=> t.Account)
            .Where(t => t.Account != null 
                        && t.Account.CustomerId == request.CustomerId)
            .Select(t => new TransactionResult
            {   
                SourceId = t.SourceId,
                Amount = t.Amount,
                Status = t.Status,
                TransactionType = t.TransactionType
            }).ToListAsync(cancellationToken);

        return transactions;
    }
}