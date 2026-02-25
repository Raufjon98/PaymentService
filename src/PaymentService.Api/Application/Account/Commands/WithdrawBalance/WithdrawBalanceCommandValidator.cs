using FluentValidation;

namespace PaymentService.Api.Application.Account.Commands.WithdrawBalance;

public class WithdrawBalanceCommandValidator : AbstractValidator<WithdrawBalanceCommand>
{
    public WithdrawBalanceCommandValidator()
    {
        RuleFor(x=>x.WithdrawRequest)
            .NotEmpty()
            .WithMessage("Enter withdraw parameters.");
        
        RuleFor(x=>x.WithdrawRequest.SourceId)
            .NotEmpty()
            .WithMessage("Invalid SourceId.");
        
        RuleFor(x=>x.WithdrawRequest.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
        
        RuleFor(x=>x.WithdrawRequest.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("Invalid customer id");
    }
}