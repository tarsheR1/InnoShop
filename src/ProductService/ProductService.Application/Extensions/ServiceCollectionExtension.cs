using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace ProductService.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();

            return services;
        }
    }

}
