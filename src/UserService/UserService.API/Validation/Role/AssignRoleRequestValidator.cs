using FluentValidation;
using UserService.API.Dto.Role;

namespace UserService.API.Validation.Role
{
    public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
    {
        public AssignRoleRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .NotEqual(Guid.Empty).WithMessage("Valid User ID is required");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters")
                .Matches("^[a-zA-Z]+$").WithMessage("Role name can only contain letters");
        }
    }
}
