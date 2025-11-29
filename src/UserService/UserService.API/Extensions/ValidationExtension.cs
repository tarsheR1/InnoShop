using FluentValidation;
using UserService.API.Dto.Auth;
using UserService.API.Dto.Role;
using UserService.API.Dto.User;
using UserService.API.Validation.Auth;
using UserService.API.Validation.Role;
using UserService.API.Validation.User;

namespace UserService.API.Extensions
{
    public static class ValidationExtension
    {
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
            services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
            services.AddScoped<IValidator<RevokeTokenRequest>, RevokeTokenRequestValidator>();
            services.AddScoped<IValidator<AssignRoleRequest>, AssignRoleRequestValidator>();
            services.AddScoped<IValidator<RemoveRoleRequest>, RemoveRoleRequestValidator>();
            services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
            services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
            services.AddScoped<IValidator<UpdateUserStatusRequest>, UpdateUserStatusRequestValidator>();

            return services;
        }
    }
}
