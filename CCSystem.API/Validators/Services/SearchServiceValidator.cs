using CCSystem.Infrastructure.DTOs.Services;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class SearchServiceValidator : AbstractValidator<SearchServiceRequest>
    {
        public SearchServiceValidator()
        {
            // Nếu ServiceName có giá trị thì kiểm tra độ dài tối đa.
            RuleFor(x => x.ServiceName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(255).WithMessage("ServiceName cannot exceed 255 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ServiceName));

            // Nếu Description có giá trị thì kiểm tra độ dài tối đa.
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            // Nếu Price có giá trị thì kiểm tra giá trị không âm.
            RuleFor(x => x.Price)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.")
                .When(x => x.Price >= 0);

            // Nếu Duration có giá trị thì kiểm tra giá trị lớn hơn 0.
            RuleFor(x => x.Duration)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.")
                .When(x => x.Duration > 0);

            // Nếu CategoryName có giá trị thì kiểm tra không rỗng và độ dài tối đa.
            RuleFor(x => x.CategoryName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("CategoryName is required.")
                .MaximumLength(255).WithMessage("CategoryName cannot exceed 255 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.CategoryName));

            //// Kiểm tra logic tùy chỉnh: Nếu có cả MinPrice và MaxPrice, MinPrice phải nhỏ hơn hoặc bằng MaxPrice.
            //RuleFor(x => x)
            //    .Custom((request, context) =>
            //    {
            //        if (request.Price < 0)
            //        {
            //            context.AddFailure("Price", "Price must be greater than or equal to 0.");
            //        }
            //    });
        }
    }
}