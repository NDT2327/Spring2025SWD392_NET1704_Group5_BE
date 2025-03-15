using CCSystem.Infrastructure.DTOs.Promotions;
using FluentValidation;

namespace CCSystem.API.Validators.Promotions
{
    public class PutPromotionValidator : AbstractValidator<PutPromotionRequest>
    {
        public PutPromotionValidator()
        {
            //RuleFor(x => x.Code)
            //    .NotEmpty().WithMessage("Promotion code is required.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount Amount must be non-negative.");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount Percent must be between 0 and 100.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate).WithMessage("Start Date must be before End Date.");

            RuleFor(x => x.EndDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("End Date must be in the future.");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Min Order Amount must be non-negative.");

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Max Discount Amount must be non-negative.");
        }
    }
}
