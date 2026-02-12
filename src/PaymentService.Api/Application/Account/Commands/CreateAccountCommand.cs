using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Events;
using PaymentService.Contracts.Account.Responses;

namespace PaymentService.Api.Application.Account.Commands;

public record CreateAccountCommand(Guid CustomerId) : IRequest<AccountResponse>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publish;

    public CreateAccountCommandHandler(ApplicationDbContext context, IPublishEndpoint publish)
    {
        _context = context;
        _publish = publish;
    }

    public async Task<AccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.CustomerId == request.CustomerId, cancellationToken);

        if (existingAccount != null)
        {
            throw new ExistsException(nameof(Account), existingAccount.Id.ToString());
        }

        Domain.Entities.AccountEntity accountEntity = new Domain.Entities.AccountEntity()
        {
            AccountNumber = Guid.NewGuid().ToString(),
            CustomerId = request.CustomerId,
        };

        await _context.Accounts.AddAsync(accountEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _publish.Publish(
            new AccountCreatedEvent()
            {
                Id = accountEntity.Id,
                CreatedOnUtc = DateTime.UtcNow,
            },
            cancellationToken);

        return new AccountResponse()
        {
            CustomerId = accountEntity.CustomerId,
            AccountNumber = accountEntity.AccountNumber,
            Balance = accountEntity.Balance,
            IsActive = accountEntity.IsActive,
        };
    }
}