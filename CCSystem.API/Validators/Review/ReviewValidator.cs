using FluentValidation;
using CCSystem.BLL.DTOs.Review;

namespace CCSystem.API.Validators.Review
{
    public class ReviewValidator : AbstractValidator<ReviewRequest>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("CustomerId phải lớn hơn 0.");

            RuleFor(x => x.DetailId)
                .GreaterThan(0).WithMessage("DetailId phải lớn hơn 0.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Nội dung review tối đa 500 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment)); // Chỉ kiểm tra khi có giá trị

            RuleFor(x => x.Rating)
    .NotNull().WithMessage("Đánh giá không được để trống.")
    .InclusiveBetween(1, 5).WithMessage("Đánh giá phải từ 1 đến 5 sao.");
        }
    }
}
