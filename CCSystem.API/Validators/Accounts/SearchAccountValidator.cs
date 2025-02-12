using CCSystem.BLL.DTOs.Accounts;
using FluentValidation;

namespace CCSystem.API.Validators.Accounts
{
    public class SearchAccountValidator : AbstractValidator<AccountSearchRequest>
    {
        public SearchAccountValidator()
        {
            // Nếu Email có giá trị thì kiểm tra định dạng email hợp lệ.
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .EmailAddress().WithMessage("{PropertyName} must be a valid email address.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // Nếu Role có giá trị thì kiểm tra không được rỗng.
            RuleFor(x => x.Role)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .When(x => !string.IsNullOrWhiteSpace(x.Role));

            // Nếu Address có giá trị thì kiểm tra không được rỗng.
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            // Nếu Phone có giá trị thì kiểm tra không được rỗng.
            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            // Nếu FullName có giá trị thì kiểm tra không được rỗng.
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            // Nếu Status có giá trị thì kiểm tra không được rỗng.
            RuleFor(x => x.Status)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

            // Nếu có cung cấp cả MinCreatedDate và MaxCreatedDate thì MinCreatedDate phải nhỏ hơn hoặc bằng MaxCreatedDate.
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    if (request.MinCreatedDate.HasValue && request.MaxCreatedDate.HasValue)
                    {
                        if (request.MinCreatedDate > request.MaxCreatedDate)
                        {
                            context.AddFailure("MinCreatedDate", "MinCreatedDate must be less than or equal to MaxCreatedDate.");
                        }
                    }
                });
        }
    }
}
