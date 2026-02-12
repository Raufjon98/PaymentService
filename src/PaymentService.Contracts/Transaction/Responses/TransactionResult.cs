using MessagePack;
using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Contracts.Transaction.Responses;

[MessagePackObject]
public record TransactionResult
{
    [Key(0)]
    public required string SourceId { get; set; }
    [Key(1)]
    public TransactionType TransactionType { get; set; }
    [Key(2)]
    public decimal Amount { get; set; }
    [Key(3)]
    public TransationStatus Status { get; set; }
}