using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.ProductClient
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task DeactivateUserProductsAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"/api/products/users/{userId}/deactivate-products",
                    new { },
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully deactivated products for user {UserId}", userId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to deactivate products for user {UserId}", userId);
                throw new Exception($"Failed to deactivate user products: {ex.Message}");
            }
        }
    }
}
