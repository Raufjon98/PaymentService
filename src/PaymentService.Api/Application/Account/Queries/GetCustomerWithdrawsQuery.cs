using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Transaction.Enum;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Application.Account.Queries;

public record GetCustomerWithdrawsQuery(Guid CustomerId) : IRequest<List<TransactionResult>>;

public class GetCustomerWithdrawsQueryHandler : IRequestHandler<GetCustomerWithdrawsQuery, List<TransactionResult>>
{
    private readonly ApplicationDbContext _context;

    public GetCustomerWithdrawsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TransactionResult>> Handle(GetCustomerWithdrawsQuery request, CancellationToken cancellationToken)
    {
        var withdraws = await _context.Transactions
            .Include(t => t.Account)
            .Where(t => t.Account != null
                        && t.Account.CustomerId == request.CustomerId
                        && t.Account.IsDeleted == false
                        && t.TransactionType == TransactionType.Expense)
            .Select(t =>  new TransactionResult
            {
                SourceId = t.SourceId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Status = t.Status,
            }).ToListAsync(cancellationToken);
        
        return withdraws;
    }
}
