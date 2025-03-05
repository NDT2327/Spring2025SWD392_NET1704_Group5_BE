using CCSystem.BLL.DTOs.BookingDetails;
using FluentValidation;

namespace CCSystem.API.Validators.BookingDetails
{
    public class PostBookingDetailRequestValidator : AbstractValidator<PostBookingDetailRequest>
    {
        public PostBookingDetailRequestValidator()
        {
            // Validate that ServiceId is greater than 0.
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");


            // Validate that Quantity is greater than 0.
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");


            // Validate that ServiceDetailId is greater than 0.
            RuleFor(x => x.ServiceDetailId)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");


        }
    }
}
