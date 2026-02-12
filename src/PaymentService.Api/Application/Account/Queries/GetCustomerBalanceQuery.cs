using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Responses;

namespace PaymentService.Api.Application.Account.Queries;

public record GetCustomerBalanceQuery(Guid CustomerId) : IRequest<BalanceOperationResponse>;

public class GetCustomerBalanceQueryHandler : IRequestHandler<GetCustomerBalanceQuery, BalanceOperationResponse>
{
    private readonly ApplicationDbContext _context;

    public GetCustomerBalanceQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BalanceOperationResponse> Handle(GetCustomerBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a => a.CustomerId == request.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.CustomerId.ToString());
        }

        return new BalanceOperationResponse
        {
            Balance = account.Balance,
            Message = $"Showing balance!"
        };
    }
}