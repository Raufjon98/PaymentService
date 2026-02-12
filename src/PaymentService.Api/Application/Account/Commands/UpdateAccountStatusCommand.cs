using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Application.Account.Queries;
using PaymentService.Api.Application.Common.Exceptions;
using PaymentService.Api.Infrastructure.Data;
using PaymentService.Contracts.Account.Events;
using PaymentService.Contracts.Account.Requests;
using PaymentService.Contracts.Account.Responses;

namespace PaymentService.Api.Application.Account.Commands;

public record UpdateAccountStatusCommand(UpdateAccountStatusRequest UpdateAccountStatusRequest)
    : IRequest<AccountResponse>;

public class UpdateAccountStatusCommandHandler : IRequestHandler<UpdateAccountStatusCommand, AccountResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateAccountStatusCommandHandler(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<AccountResponse> Handle(UpdateAccountStatusCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a => a.CustomerId == request.UpdateAccountStatusRequest.CustomerId && a.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Account), request.UpdateAccountStatusRequest.CustomerId.ToString());
        }

        if (account.IsActive == request.UpdateAccountStatusRequest.IsActive)
        {
            throw new Exception(
                $"Account is already {(request.UpdateAccountStatusRequest.IsActive ? "active" : "inActive")}!");
        }

        account.IsActive = request.UpdateAccountStatusRequest.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        
        await _publishEndpoint.Publish(
            new AccountUpdatedEvent()
            {
                Id = account.Id,
                UpdatedOnUtc = DateTime.UtcNow
            },
            cancellationToken);

        return new AccountResponse
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CustomerId = account.CustomerId
        };
    }
}