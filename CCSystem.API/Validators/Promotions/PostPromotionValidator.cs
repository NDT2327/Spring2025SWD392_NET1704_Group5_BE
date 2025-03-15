using CCSystem.Infrastructure.DTOs.Promotions;
using FluentValidation;
using System;

namespace CCSystem.API.Validators.Promotions
{
    public class PostPromotionValidator : AbstractValidator<PostPromotionRequest>
    {
        public PostPromotionValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Promotion code cannot be empty.")
                .MaximumLength(50).WithMessage("Promotion code cannot exceed 50 characters.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount amount must be 0 or greater.")
                .When(x => x.DiscountAmount.HasValue);

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.")
                .When(x => x.DiscountPercent.HasValue);

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount must be 0 or greater.")
                .When(x => x.MinOrderAmount.HasValue);

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum discount amount must be 0 or greater.")
                .When(x => x.MaxDiscountAmount.HasValue);
        }
    }
}
