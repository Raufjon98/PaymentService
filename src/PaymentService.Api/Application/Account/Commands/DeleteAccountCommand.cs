using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Events;

namespace PaymentService.Api.Application.Account.Commands;

public record DeleteAccountCommand(Guid CustomerId) : IRequest<bool>;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteAccountCommandHandler(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a => a.CustomerId == request.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.CustomerId.ToString());
        }

        account.IsDeleted = true;
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        if (result > 0)
        {
            await _publishEndpoint.Publish(
                new AccountDeletedEvent()
                {
                    Id = account.Id,
                    DeletedOnUtc = DateTime.UtcNow
                },
                cancellationToken);
            return true;
        }

        return false;
    }
}