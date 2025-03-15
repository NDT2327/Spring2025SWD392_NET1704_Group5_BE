using CCSystem.Infrastructure.DTOs.ServiceDetails;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class PostServiceDetailValidator : AbstractValidator<PostServiceDetailRequest>
    {
        public PostServiceDetailValidator()
        {
            RuleFor(x => x.ServiceId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.OptionName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(255).WithMessage("{PropertyName} cannot exceed 255 characters.");

            RuleFor(x => x.OptionType)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.");

            RuleFor(x => x.BasePrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be 0 or greater.");

            RuleFor(x => x.Unit)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

            RuleFor(x => x.Duration)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0 minutes.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(500).WithMessage("{PropertyName} cannot exceed 500 characters.");

            RuleFor(x => x.IsActive)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.");
        }
    }
}
