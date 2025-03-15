using CCSystem.Infrastructure.DTOs.Category;
using FluentValidation;

namespace CCSystem.API.Validators.Categories
{
    public class CategoryIdValidator : AbstractValidator<CategoryIdRequest>
    {
        public CategoryIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
