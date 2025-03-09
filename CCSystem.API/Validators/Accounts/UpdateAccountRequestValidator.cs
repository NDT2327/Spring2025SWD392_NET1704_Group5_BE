using CCSystem.BLL.DTOs.Accounts;
using FluentValidation;

namespace CCSystem.API.Validators.Accounts
{
    public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.Address)
               .NotNull().WithMessage("{PropertyName} is required.")  // Bắt buộc không được null
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")  // Không được rỗng
                .MaximumLength(255).WithMessage("{PropertyName} cannot exceed 255 characters.");

            RuleFor(x => x.Phone)
                  .NotNull().WithMessage("{PropertyName} is required.")
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("{PropertyName} is not a valid phone number.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(100).WithMessage("{PropertyName} cannot exceed 100 characters.")
                .When(x => string.IsNullOrWhiteSpace(x.FullName)); // Chỉ bắt lỗi nếu FullName chưa có dữ liệu

            RuleFor(x => x.Avatar)
                .Must(file => file == null || file.Length > 0)
                .WithMessage("Avatar file cannot be empty.")
                .When(x => x.Avatar != null); // Chỉ kiểm tra nếu Avatar được gửi lên

            RuleFor(x => x.Gender)
               .NotNull().WithMessage("{PropertyName} is required.") // Không cho phép null
                .Must(x => string.IsNullOrEmpty(x) || x == "Male" || x == "Female" || x == "Other")
                .WithMessage("{PropertyName} must be 'Male', 'Female', or 'Other'.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("{PropertyName} must be in the past.")
                .When(x => x.DateOfBirth.HasValue); // Chỉ kiểm tra nếu có DateOfBirth

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5).WithMessage("{PropertyName} must be between 0 and 5.")
                .When(x => x.Rating.HasValue); // Chỉ kiểm tra nếu Rating không null

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.")
                .When(x => x.Experience.HasValue); // Chỉ kiểm tra nếu Experience không null

            RuleFor(x => x.Status)
                  .NotNull().WithMessage("{PropertyName} is required.")
                .Must(x => x == "Active" || x == "Inactive")
                .WithMessage("{PropertyName} must be 'Active' or 'Inactive'.");
        }
    }
}
