using CCSystem.BLL.DTOs.Accounts;
using FluentValidation;

namespace CCSystem.API.Validators.Authentications
{
    public class LoginAccountValidator : AbstractValidator<AccountLoginRequest>
    {
        public LoginAccountValidator()
        {
            RuleFor(ar => ar.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email Format.");

            RuleFor(ar => ar.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}
