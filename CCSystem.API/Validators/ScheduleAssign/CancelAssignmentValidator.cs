using CCSystem.Infrastructure.DTOs.ScheduleAssign;
using FluentValidation;

namespace CCSystem.API.Validators.ScheduleAssign
{
	public class CancelAssignmentValidator : AbstractValidator<CancelAssignmentRequest>
	{
		public CancelAssignmentValidator()
		{
			RuleFor(x => x.AssignmentId)
				.NotEmpty().WithMessage("{PropertyName} is not empty.")
				.NotNull().WithMessage("{PropertyName} is not null.")
				.GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("{PropertyName} must not be empty.")
				.MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.");
		}
	}
}
