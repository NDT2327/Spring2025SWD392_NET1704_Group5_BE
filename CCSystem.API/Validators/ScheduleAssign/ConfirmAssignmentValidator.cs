using CCSystem.BLL.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
    public class ConfirmAssignmentValidator : AbstractValidator<ConfirmAssignmentRequest>
    {
        public ConfirmAssignmentValidator()
        {
            RuleFor(x => x.BookingDetailId)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
        }
    }
}
