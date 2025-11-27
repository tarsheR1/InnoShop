using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces;
using UserService.Application.Models.Settings;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Services;

namespace UserService.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, Services.UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
