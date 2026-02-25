using FluentValidation;

namespace PaymentService.Api.Application.Account.Queries.GetCustomerBalance;

public class GetCustomerBalanceQueryValidator :  AbstractValidator<GetCustomerBalanceQuery>
{
    public GetCustomerBalanceQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");
    }
}