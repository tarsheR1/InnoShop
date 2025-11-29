using FluentValidation;

namespace ProductService.Api.Validations.Category
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("ID категории обязательно");

            RuleFor(x => x.Name)
                .Length(2, 100).WithMessage("Название категории должно быть от 2 до 100 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-]+$").WithMessage("Название категории содержит недопустимые символы")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.Description))
                .WithMessage("Хотя бы одно поле должно быть заполнено для обновления");
        }
    }
}
