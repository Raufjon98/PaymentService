using MessagePack;

namespace PaymentService.Contracts.Account.Requests;

[MessagePackObject]
public record UpdateAccountStatusRequest
{
    [Key(0)]
    public required Guid CustomerId { get; set; }
    [Key(1)]
    public bool IsActive { get; set; }
}