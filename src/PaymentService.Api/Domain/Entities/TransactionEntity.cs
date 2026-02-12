using PaymentService.Contracts.Transaction.Enum;

namespace PaymentService.Api.Domain.Entities;

public class TransactionEntity : BaseAuditableEntity
{
    public required string SourceId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public TransationStatus Status { get; set; }
    public Guid AccountId { get; set; }
    public AccountEntity? Account { get; set; }
}