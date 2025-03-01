using CCSystem.BLL.DTOs.Promotions;
using FluentValidation;
using System;

namespace CCSystem.API.Validators.Promotions
{
    public class PutPromotionValidator : AbstractValidator<PutPromotionRequest>
    {
        public PutPromotionValidator()
        {
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} cannot exceed 50 characters.");

            RuleFor(x => x.DiscountAmount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
                .When(x => x.DiscountAmount.HasValue);

            RuleFor(x => x.DiscountPercent)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} must be between 0 and 100.")
                .When(x => x.DiscountPercent.HasValue);

            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("{PropertyName} cannot be in the past.");

            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(x => x.StartDate).WithMessage("{PropertyName} must be after StartDate.");

            RuleFor(x => x.MinOrderAmount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
                .When(x => x.MinOrderAmount.HasValue);

            RuleFor(x => x.MaxDiscountAmount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than or equal to 0.")
                .When(x => x.MaxDiscountAmount.HasValue);
        }
    }
}
