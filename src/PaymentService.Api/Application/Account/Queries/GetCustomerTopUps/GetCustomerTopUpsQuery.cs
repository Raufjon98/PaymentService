using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Transaction.Enum;
using PaymentService.Contracts.Transaction.Responses;

namespace PaymentService.Api.Application.Account.Queries.GetCustomerTopUps;

public record GetCustomerTopUpsQuery(Guid CustomerId) : IRequest<List<TransactionResult>>;

public class GetCustomerTopUpsQueryHandler : IRequestHandler<GetCustomerTopUpsQuery, List<TransactionResult>>
{
    private readonly ApplicationDbContext _context;

    public GetCustomerTopUpsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionResult>> Handle(GetCustomerTopUpsQuery request,
        CancellationToken cancellationToken)
    {
        var topUps = await _context.Transactions
            .Include(t => t.Account)
            .Where(t => t.Account != null
                        && t.Account.CustomerId == request.CustomerId
                        && t.TransactionType == TransactionType.TopUp)
            .Select(t => new TransactionResult
            {
                Amount = t.Amount,
                Status = t.Status,
                SourceId = t.SourceId,
                TransactionType = t.TransactionType,
            }).ToListAsync(cancellationToken);

        return topUps;
    }
}