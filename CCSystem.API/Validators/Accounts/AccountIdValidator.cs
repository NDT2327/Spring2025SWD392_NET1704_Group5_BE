using CCSystem.Infrastructure.DTOs.Accounts;
using FluentValidation;

namespace CCSystem.API.Validators.Accounts
{
    public class AccountIdValidator : AbstractValidator<AccountIdRequest>
    {
        public AccountIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
