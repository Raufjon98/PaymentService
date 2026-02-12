using MessagePack;

namespace PaymentService.Contracts.Account.Responses;

[MessagePackObject]
public record AccountResponse
{
    [Key(0)]
    public required Guid CustomerId { get; set; }
    [Key(1)]
    public string? AccountNumber { get; set; }
    [Key(2)]
    public decimal Balance { get; set; }
    [Key(3)]
    public bool IsActive { get; set; }
}