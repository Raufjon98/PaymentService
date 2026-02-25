using FluentValidation;

namespace PaymentService.Api.Application.Account.Queries.GetCustomerTopUps;

public class GetCustomerTopUpsQueryValidator : AbstractValidator<GetCustomerTopUpsQuery>
{
    public GetCustomerTopUpsQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");
    }
}