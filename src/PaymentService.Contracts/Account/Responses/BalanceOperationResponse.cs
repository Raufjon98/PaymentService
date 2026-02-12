using MessagePack;

namespace PaymentService.Contracts.Account.Responses;

[MessagePackObject]
public record BalanceOperationResponse
{
    [Key(0)]
    public decimal Balance { get; set; }
    [Key(1)]
    public string Message { get; set; } = string.Empty;
}