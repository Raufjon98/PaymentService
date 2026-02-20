namespace PaymentService.Contracts.Account.Events;

public class WithDrawalEvent
{
    public string SourceId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}