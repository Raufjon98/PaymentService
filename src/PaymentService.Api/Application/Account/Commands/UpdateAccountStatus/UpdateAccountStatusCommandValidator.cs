using FluentValidation;

namespace PaymentService.Api.Application.Account.Commands.UpdateAccountStatus;

public class UpdateAccountStatusCommandValidator : AbstractValidator<UpdateAccountStatusCommand>
{
    public UpdateAccountStatusCommandValidator()
    {
        RuleFor(x=>x.UpdateAccountStatusRequest.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");;
    }
}