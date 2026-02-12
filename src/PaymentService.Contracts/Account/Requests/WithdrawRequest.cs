using MessagePack;

namespace PaymentService.Contracts.Account.Requests;

[MessagePackObject]
public record WithdrawRequest : IPaymentTransactionRequest
{
    [Key(0)]
    public required Guid CustomerId { get; set; }
    [Key(1)]
    public required string SourceId { get; set; }
    [Key(2)]
    public required decimal Amount { get; set; }
}