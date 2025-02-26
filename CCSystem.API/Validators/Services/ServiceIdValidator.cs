using CCSystem.BLL.DTOs.Services;
using FluentValidation;

namespace CCSystem.API.Validators.Services
{
    public class ServiceIdValidator : AbstractValidator<ServiceIdRequest>
    {
        public ServiceIdValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.")
                .NotNull().WithMessage("{PropertyName} is not null.");
        }
    }
}
