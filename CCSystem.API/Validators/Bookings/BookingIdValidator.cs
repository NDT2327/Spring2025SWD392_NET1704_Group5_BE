using CCSystem.Infrastructure.DTOs.Bookings;
using FluentValidation;

namespace CCSystem.API.Validators.Bookings
{
    public class BookingIdValidator : AbstractValidator<BookingIdRequest>
    {
        public BookingIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
