using FluentValidation;
using UserService.API.Dto.User;

namespace UserService.API.Validation.User
{
    public class UpdateUserStatusRequestValidator : AbstractValidator<UpdateUserStatusRequest>
    {
        public UpdateUserStatusRequestValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive status is required");
        }
    }
}
