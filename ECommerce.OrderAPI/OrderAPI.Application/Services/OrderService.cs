using System.Net.Http.Json;
using OrderAPI.Application.DTOs;
using OrderAPI.Application.DTOs.Conversions;
using OrderAPI.Application.Interfaces;
using Polly;
using Polly.Registry;

namespace OrderAPI.Application.Services
{
    public class OrderService(IOrderRepository orderRepository,
        HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        // GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product API using HttpClient
            // Redirect this call to the API Gateway since product API is not response to outsiders.
            var getProduct = await httpClient.GetAsync($"/api/Product/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/Products/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        // GET ORDER DETAILS BY ID
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare order
            var order = await orderRepository.FindByIdAsync(orderId);
            if (order == null || order!.Id <= 0) 
                return null!;
            // Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate order details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO?.Name,
                appUserDTO?.Email,
                appUserDTO?.Address,
                appUserDTO?.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        // GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders
            var orders = await orderRepository.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any()) return null!;

            // convert from entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
