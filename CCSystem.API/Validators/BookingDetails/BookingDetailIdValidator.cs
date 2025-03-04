using CCSystem.BLL.DTOs.BookingDetails;
using FluentValidation;

namespace CCSystem.API.Validators.BookingDetails
{
    public class BookingDetailIdValidator : AbstractValidator<BookingDetailIdRequest>
    {
        public BookingDetailIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
