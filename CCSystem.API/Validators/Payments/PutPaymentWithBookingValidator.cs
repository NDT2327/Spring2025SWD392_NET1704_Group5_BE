using CCSystem.BLL.DTOs.Payments;
using CCSystem.DAL.Enums;
using FluentValidation;

namespace CCSystem.API.Validators.Payments
{
    public class PutPaymentWithBookingValidator : AbstractValidator<PutPaymentWithBooking>
    {
        public PutPaymentWithBookingValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method cannot exceed 50 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("Transaction ID is required.")
                .MaximumLength(100).WithMessage("Transaction ID cannot exceed 100 characters.");
        }

    }
}
