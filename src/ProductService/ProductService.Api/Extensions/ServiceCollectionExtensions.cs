using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Hellang.Middleware.ProblemDetails;
using ProductService.Api.Validations.Category;

namespace ProductService.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();

            return services;
        }
    }
}
