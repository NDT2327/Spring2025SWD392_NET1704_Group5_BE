using CCSystem.Infrastructure.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
	public class ConfirmCancelAssignmentValidator : AbstractValidator<ConfirmCancelAssignmentRequest>
	{
		public ConfirmCancelAssignmentValidator()
		{
			RuleFor(x => x.AssignmentId)
				.NotEmpty().WithMessage("{PropertyName} is not empty.")
				.NotNull().WithMessage("{PropertyName} is not null.")
				.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

			RuleFor(x => x.IsApproved)
				.NotNull().WithMessage("{PropertyName} must not be null.");
		}
	}
}
