using CCSystem.Infrastructure.DTOs.ServiceDetails;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class PutServiceDetailValidator : AbstractValidator<PutServiceDetailRequest>
    {
        public PutServiceDetailValidator()
        {
            //RuleFor(x => x.ServiceDetailId)
            //               .GreaterThan(0).WithMessage("ServiceDetailId must be greater than 0.");

            RuleFor(x => x.ServiceId)
                .GreaterThanOrEqualTo(0).WithMessage("ServiceId must be a non-negative number.");

            RuleFor(x => x.OptionName)
                .NotEmpty().WithMessage("OptionName cannot be empty.")
                .MaximumLength(255).WithMessage("OptionName cannot exceed 255 characters.");

            RuleFor(x => x.OptionType)
                .NotEmpty().WithMessage("OptionType cannot be empty.")
                .MaximumLength(100).WithMessage("OptionType cannot exceed 100 characters.");

            RuleFor(x => x.BasePrice)
                .GreaterThanOrEqualTo(0).WithMessage("BasePrice must be 0 or greater.");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Unit cannot be empty.")
                .MaximumLength(50).WithMessage("Unit cannot exceed 50 characters.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive cannot be null.");
        }
    }
}
