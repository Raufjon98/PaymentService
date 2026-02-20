using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Api.Domain.Entities;

namespace PaymentService.Api.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x=>x.CustomerId).IsRequired();
        builder.Property(x=>x.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
     
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}