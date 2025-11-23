using FluentValidation;

namespace ProductService.Api.Validations.Product.Query
{
    public class GetUserProductsQueryValidator : AbstractValidator<GetUserProductsQuery>
    {
        public GetUserProductsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ID пользователя обязательно");
        }
    }
}
