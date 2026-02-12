namespace PaymentService.Contracts.Account.Events;

public record BalanceDecreaseEvent
{
    public Guid Id { get; init; }
    public  Guid TransactionId { get; init; }
    public decimal Amount { get; init; }
    public decimal CurrentBalance { get; init; }
    public DateTime DecreasedOnUtc { get; init; }
}