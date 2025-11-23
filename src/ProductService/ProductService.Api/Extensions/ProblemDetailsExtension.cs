using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;

namespace ProductService.Api.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.Map<ArgumentException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                });

                options.Map<UnauthorizedAccessException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                });

                options.Map<InvalidOperationException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Invalid operation",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                });

                options.Map<FileNotFoundException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Resource not found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                });

                options.Map<NotImplementedException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Not implemented",
                    Detail = ex.Message,
                    Status = StatusCodes.Status501NotImplemented,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2"
                });

                options.Map<System.Security.SecurityException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = ex.Message,
                    Status = StatusCodes.Status403Forbidden,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                });

                options.Map<InvalidDataException>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Conflict",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
                });

                options.Map<Exception>(ex => new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                });
            });

            return services;
        }
    }
}
