using FluentValidation;

namespace PaymentService.Api.Application.Account.Commands.TopUpBalance;

public class TopUpBalanceCommandValidator : AbstractValidator<TopUpBalanceCommand>
{
    public TopUpBalanceCommandValidator()
    {
        RuleFor(x => x.TopUpRequest)
            .NotNull()
            .WithMessage("Enter TopUp  parameters");
        
        RuleFor(x=>x.TopUpRequest.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
        
        RuleFor(x => x.TopUpRequest.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("CustomerId is required.")
            .NotEqual(Guid.Empty).WithMessage("CustomerId is required.");
        
        RuleFor(x=>x.TopUpRequest.SourceId).
            Cascade(CascadeMode.Stop)
            .Must(x=>Guid.TryParse(x, out _))
            .WithMessage("SourceId is invalid");
    }
}