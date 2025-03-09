using CCSystem.BLL.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
    public class AssignIdValidator : AbstractValidator<AssignIdRequest>
    {
        public AssignIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
