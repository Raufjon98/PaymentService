namespace PaymentService.Contracts.Account.Events;

public record AccountDeletedEvent
{
    public Guid Id { get; init; }
    public DateTime DeletedOnUtc { get; init; }
}