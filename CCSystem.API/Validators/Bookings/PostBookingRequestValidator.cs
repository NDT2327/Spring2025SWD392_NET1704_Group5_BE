using CCSystem.API.Validators.BookingDetails;
using CCSystem.BLL.DTOs.Bookings;
using FluentValidation;

namespace CCSystem.API.Validators.Bookings
{
    public class PostBookingRequestValidator : AbstractValidator<PostBookingRequest>
    {
        public PostBookingRequestValidator()
        {
            // Validate that CustomerId is greater than 0.
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .NotNull().WithMessage("{PropertyName} must not be null.");

            // Validate that Notes is not empty.
            RuleFor(x => x.Notes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.");

            // Validate that PaymentMethod is not empty.
            RuleFor(x => x.PaymentMethod)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.");

            // Validate that Address is not empty.
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.");

            // Validate that BookingDetails is not null and contains at least one element.
            RuleFor(x => x.BookingDetails)
                .NotNull().WithMessage("{PropertyName} must not be null.")
                .Must(details => details.Any()).WithMessage("{PropertyName} must contain at least one booking detail.");

            // Apply validator for each element in BookingDetails.
            RuleForEach(x => x.BookingDetails)
                .SetValidator(new PostBookingDetailRequestValidator());
        }
    }
}
