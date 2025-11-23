using FluentValidation;

namespace ProductService.Api.Validations.Product
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ID продукта обязательно");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ID пользователя обязательно");
        }
    }

}
