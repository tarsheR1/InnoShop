using FluentValidation;

namespace ProductService.Api.Validations.Product
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название продукта обязательно")
                .Length(2, 200).WithMessage("Название продукта должно быть от 2 до 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0")
                .LessThan(10000000).WithMessage("Цена не может превышать 10,000,000");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("ID категории обязательно");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ID пользователя обязательно");
        }
    }
}
