using FluentValidation;

namespace PaymentService.Api.Application.Account.Queries.GetCustomerWithdraws;

public class GetCustomerWithdrawsQueryValidator : AbstractValidator<GetCustomerWithdrawsQuery>
{
    public GetCustomerWithdrawsQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");
    }
}