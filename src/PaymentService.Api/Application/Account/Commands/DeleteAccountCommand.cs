using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;

namespace PaymentService.Api.Application.Account.Commands;

public record DeleteAccountCommand(Guid CustomerId) : IRequest<bool>;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public DeleteAccountCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a=>a.CustomerId == request.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.CustomerId.ToString());
        }
        
        account.IsDeleted = true;
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}