using FluentValidation;

namespace PaymentService.Api.Application.Account.Commands.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.")
            .NotNull().WithMessage("CustomerId is required.");
    }    
}