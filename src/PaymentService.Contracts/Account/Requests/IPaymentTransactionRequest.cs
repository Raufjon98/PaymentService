
namespace PaymentService.Contracts.Account.Requests;

public interface IPaymentTransactionRequest
{
    public Guid CustomerId { get; set; }
    public string SourceId { get; set; }
    public decimal Amount { get; set; }
}