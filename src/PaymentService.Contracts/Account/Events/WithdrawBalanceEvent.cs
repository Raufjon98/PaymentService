using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Contracts.Account.Events;

public class WithdrawBalanceEvent
{
    public string SourceId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public TransationStatus TransactionStatus { get; set; }
}