using FluentValidation;
using UserService.API.Dto.User;

namespace UserService.API.Validation.User
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .Matches("^[a-zA-Zа-яА-ЯёЁ]+$").WithMessage("First name can only contain letters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .Matches("^[a-zA-Zа-яА-ЯёЁ]+$").WithMessage("Last name can only contain letters")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Valid email address is required")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }

}
