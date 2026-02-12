namespace PaymentService.Contracts.Account.Events;

public record AccountUpdatedEvent
{
    public Guid Id { get; init; }
    public DateTime UpdatedOnUtc { get; init; }
}