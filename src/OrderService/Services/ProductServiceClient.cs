using OrderService.Models;

namespace OrderService.Services;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Product {ProductId} not found", productId);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId}", productId);
            throw;
        }
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/stock/{productId}?quantity={-quantity}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
            throw;
        }
    }
}
