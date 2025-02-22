using CCSystem.BLL.DTOs.Services;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class CreateServiceValidator : AbstractValidator<PostServiceRequest>
    {
        public CreateServiceValidator()
        {
            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.ServiceName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(255).WithMessage("{PropertyName} cannot exceed 255 characters.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.");

            RuleFor(x => x.Image)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.");

            RuleFor(x => x.Price)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.Duration)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0 minutes.");

            RuleFor(x => x.IsActive)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.");
        }
    }
}
