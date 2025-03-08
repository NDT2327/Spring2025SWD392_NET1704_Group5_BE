using CCSystem.BLL.DTOs.Services;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class UpdateServiceValidator : AbstractValidator<PostServiceRequest>
    {
        public UpdateServiceValidator()
        {
            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");  // Tùy thuộc vào yêu cầu, có thể bỏ điều kiện này khi cập nhật

            RuleFor(x => x.ServiceName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(255).WithMessage("{PropertyName} cannot exceed 255 characters.");
            // Không cần kiểm tra NotEmpty() hoặc NotNull() ở đây vì ServiceName có thể không thay đổi khi cập nhật.

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.");

            RuleFor(x => x.Image)
       .Cascade(CascadeMode.StopOnFirstFailure)
       .NotNull().WithMessage("{PropertyName} cannot be null.")
       .When(x => x.Image != null);

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
