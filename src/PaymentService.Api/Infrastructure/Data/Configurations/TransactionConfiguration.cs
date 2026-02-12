using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Api.Domain.Entities;

namespace PaymentService.Api.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}