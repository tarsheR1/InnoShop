using FluentValidation;

namespace ProductService.Api.Validations.Product.Query
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ID продукта обязательно");
        }
    }

}
