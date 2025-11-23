using FluentValidation;

namespace ProductService.Api.Validations.Product
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ID продукта обязательно");

            RuleFor(x => x.Name)
                .Length(2, 200).WithMessage("Название продукта должно быть от 2 до 200 символов")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0")
                .LessThan(10000000).WithMessage("Цена не может превышать 10,000,000")
                .When(x => x.Price.HasValue);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.Name) ||
                          !string.IsNullOrEmpty(x.Description) ||
                          x.Price.HasValue ||
                          x.IsAvailable.HasValue ||
                          x.CategoryId.HasValue)
                .WithMessage("Хотя бы одно поле должно быть заполнено для обновления");
        }
    }
}
