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

            RuleFor(x => x.Year)
        .NotNull().WithMessage("Year is required.")
        .InclusiveBetween(1900, DateTime.UtcNow.Year).WithMessage("Year must be between 1900 and the current year.")
        .When(x => x.Month.HasValue || x.Day.HasValue); // Bắt buộc nếu Month hoặc Day có giá trị

            RuleFor(x => x.Month)
                .NotNull().WithMessage("Month is required.")
                .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.")
                .When(x => x.Year.HasValue || x.Day.HasValue); // Bắt buộc nếu Year hoặc Day có giá trị

            RuleFor(x => x.Day)
                .NotNull().WithMessage("Day is required.")
                .InclusiveBetween(1, 31).WithMessage("Day must be between 1 and 31.")
                .When(x => x.Year.HasValue || x.Month.HasValue); // Bắt buộc nếu Year hoặc Month có giá trị
            RuleFor(x => x)
                        .Must(x => IsValidDate(x.Year, x.Month, x.Day))
                        .WithMessage("Invalid date of birth.")
                        .When(x => x.Year.HasValue && x.Month.HasValue && x.Day.HasValue);




            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5).WithMessage("{PropertyName} must be between 0 and 5.")
                .When(x => x.Rating.HasValue); // Chỉ kiểm tra nếu Rating không null

            RuleFor(x => x.Experience)
     .NotNull().WithMessage("{PropertyName} is required.") // Bắt buộc nhập
     .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");

            RuleFor(x => x.Status)
                  .NotNull().WithMessage("{PropertyName} is required.")
                .Must(x => x == "Active" || x == "Inactive")
                .WithMessage("{PropertyName} must be 'Active' or 'Inactive'.");
        }
        private bool IsValidDate(int? year, int? month, int? day)
        {
            if (year.HasValue && month.HasValue && day.HasValue)
            {
                try
                {
                    _ = new DateOnly(year.Value, month.Value, day.Value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
