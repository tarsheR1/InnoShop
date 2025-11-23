using FluentValidation;

namespace ProductService.Api.Validations.Category
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название категории обязательно")
                .Length(2, 100).WithMessage("Название категории должно быть от 2 до 100 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-]+$").WithMessage("Название категории содержит недопустимые символы");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
