namespace PaymentService.Contracts.Account.Events;

public record BalanceIncreaseEvent
{
    public Guid Id { get; init; }
    public Guid TransactionId { get; init; }
    public decimal Amount { get; init; }
    public decimal CurrentBalance { get; init; }
    public DateTime IncreasedOnUtc { get; init; }
}