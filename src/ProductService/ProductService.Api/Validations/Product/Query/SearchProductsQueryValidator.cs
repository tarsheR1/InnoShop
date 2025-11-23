using FluentValidation;

namespace ProductService.Api.Validations.Product.Query
{
    public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
    {
        public SearchProductsQueryValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Минимальная цена не может быть отрицательной")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Максимальная цена не может быть отрицательной")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MaxPrice >= x.MinPrice)
                .WithMessage("Максимальная цена должна быть больше или равна минимальной");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Поисковый запрос не должен превышать 100 символов")
                .When(x => !string.IsNullOrEmpty(x.Name));
        }
    }
}
