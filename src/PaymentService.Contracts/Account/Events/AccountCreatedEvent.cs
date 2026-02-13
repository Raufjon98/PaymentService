namespace PaymentService.Contracts.Account.Events;

public record AccountCreatedEvent
{
    public Guid Id { get; init; }
    public DateTime CreatedOnUtc { get; init; }
}