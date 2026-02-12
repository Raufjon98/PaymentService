using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Responses;

namespace PaymentService.Api.Application.Account.Commands;

public record CreateAccountCommand(Guid CustomerId) : IRequest<AccountResponse>;  

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountResponse>
{
    private readonly ApplicationDbContext _context;

    public CreateAccountCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<AccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var existingAccount = await _context.Accounts
            .Where(a => a.CustomerId == request.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingAccount != null)
        {
           throw new ExistsException(nameof(Account), existingAccount.Id.ToString());
        }
        Domain.Entities.AccountEntity accountEntity = new Domain.Entities.AccountEntity()
        {
            AccountNumber = Random.Shared.NextInt64(1000000000000, 9999999999999).ToString(),
            CustomerId = request.CustomerId,
        };
        
        await _context.Accounts.AddAsync(accountEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new AccountResponse()
        {
            CustomerId = accountEntity.CustomerId,
            AccountNumber = accountEntity.AccountNumber,
            Balance = accountEntity.Balance,
            IsActive = accountEntity.IsActive,
        };
    }
}