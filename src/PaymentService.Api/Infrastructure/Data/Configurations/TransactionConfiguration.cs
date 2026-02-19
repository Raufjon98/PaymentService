using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Api.Domain.Entities;

namespace PaymentService.Api.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceId)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(x => x.AccountId).IsRequired();
        builder.HasIndex(x => x.AccountId);
        
        builder.HasOne(x => x.Account)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}