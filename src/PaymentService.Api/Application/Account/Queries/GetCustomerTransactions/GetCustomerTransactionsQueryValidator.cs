using FluentValidation;

namespace PaymentService.Api.Application.Account.Queries.GetCustomerTransactions;

public class GetCustomerTransactionsQueryValidator: AbstractValidator<GetCustomerTransactionsQuery>
{
    public GetCustomerTransactionsQueryValidator()
    {   
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");
    }
}