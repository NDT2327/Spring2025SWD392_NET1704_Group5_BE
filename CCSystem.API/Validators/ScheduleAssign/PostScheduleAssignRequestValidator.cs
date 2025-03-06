using CCSystem.BLL.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
    public class PostScheduleAssignRequestValidator : AbstractValidator<PostScheduleAssignRequest>
    {
        public PostScheduleAssignRequestValidator()
        {
            RuleFor(x => x.HousekeeperId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");

            RuleFor(x => x.DetailId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");

            RuleFor(x => x.Notes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(500).WithMessage("{PropertyName} cannot exceed 500 characters.");
        }
    }
}
