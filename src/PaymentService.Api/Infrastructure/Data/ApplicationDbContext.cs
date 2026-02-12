using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Domain.Entities;

namespace PaymentService.Api.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
    
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
    
}