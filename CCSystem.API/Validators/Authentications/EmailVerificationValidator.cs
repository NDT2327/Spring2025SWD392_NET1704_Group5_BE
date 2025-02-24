using CCSystem.BLL.DTOs.Accounts;
using FluentValidation;

namespace CCSystem.API.Validators.Authentications
{
    public class EmailVerificationValidator : AbstractValidator<EmailVerificationRequest>
    {
        public EmailVerificationValidator()
        {
            RuleFor(ev => ev.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .EmailAddress().WithMessage("{PropertyName} is invalid Email format.");
        }
    }
}
