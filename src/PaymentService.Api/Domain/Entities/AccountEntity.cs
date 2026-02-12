namespace PaymentService.Api.Domain.Entities;

public class AccountEntity : BaseAuditableEntity
{
    public required string AccountNumber { get; set; } 
    public required Guid CustomerId { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
}