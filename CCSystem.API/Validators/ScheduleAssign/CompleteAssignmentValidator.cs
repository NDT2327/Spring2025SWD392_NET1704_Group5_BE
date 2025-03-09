using CCSystem.BLL.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
    public class CompleteAssignmentValidator : AbstractValidator<CompleteAssignmentRequest>
    {
        public CompleteAssignmentValidator()
        {
            RuleFor(x => x.AssignmentId)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.HousekeeperId)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
        }
    }
}
