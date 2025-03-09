using CCSystem.BLL.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
    public class PatchAssignStatusValidator : AbstractValidator<PatchAssignStatusRequest>
    {
        public PatchAssignStatusValidator()
        {
            RuleFor(x => x.AssignmentId)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
