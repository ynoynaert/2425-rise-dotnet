using System.Net.Http.Json;
using Azure;
using Microsoft.Extensions.Logging;
using Rise.Shared.Helpers;
using Rise.Shared.Orders;

namespace Rise.Client.Orders;

public class OrderService(HttpClient httpClient) : IOrderService
{

    public async Task<bool> CheckIfQuoteHasOrder(string orderNumber)
    {
        return await httpClient.GetFromJsonAsync<bool>($"order/HasOrder/{orderNumber}");
    }

    public async Task<OrderDto.Index> CreateOrderAsync(OrderDto.Create orderDto)
    {
        var response = await httpClient.PostAsJsonAsync("order", orderDto);
        var order = await response.Content.ReadFromJsonAsync<OrderDto.Index>();
        return order!;
    }    

    public async Task<OrderDto.Detail> GetOrderAsync(string orderNumber)
    {
        var response = await httpClient.GetFromJsonAsync<OrderDto.Detail>($"order/{orderNumber}");
        return response!;
    }    

    public async Task<OrderDto.Detail> GetOrderForSalespersonAsync(string userId, string orderNumber)
    {
        var response = await httpClient.GetFromJsonAsync<OrderDto.Detail>($"order/{orderNumber}");
        return response!;
    }    

    public async Task<IEnumerable<OrderDto.Index>> GetOrdersAsync(OrderQueryObject query)
    {
        string url = $"order?Search={query.Search}&Before={query.Before?.ToString("yyyy-MM-dd")}&After={query.After?.ToString("yyyy-MM-dd")}&SortBy={query.SortBy}&IsDescending={query.IsDescending}&PageNumber={query.PageNumber}&PageSize={query.PageSize}&Status={query.Status}";
        var response = await httpClient.GetFromJsonAsync<IEnumerable<OrderDto.Index>>(url);
        return response ?? Enumerable.Empty<OrderDto.Index>();
    }    

    public async Task<IEnumerable<OrderDto.Index>> GetOrdersForSalespersonAsync(string userId)
    {
        var response = await httpClient.GetFromJsonAsync<IEnumerable<OrderDto.Index>>($"order/");
        return response ?? Enumerable.Empty<OrderDto.Index>();
    }    

    public Task<int> GetTotalOrders()
    {
        return httpClient.GetFromJsonAsync<int>("order/Total");
    }

    public async Task<OrderDto.Detail> UpdateOrderAsync(string orderNumber, OrderDto.Update order)
    {
        
        var respons = await httpClient.PutAsJsonAsync($"order/{orderNumber}", order);
        var newOrder = await respons.Content.ReadFromJsonAsync<OrderDto.Detail>();
        return newOrder!;
    }

    public Task<OrderDto.Android> GetOrderForSalespersonAsyncAndroid(string userId, string orderNumber)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDto.Index>> GetOrdersForSalespersonAsync(string userId, OrderQueryObject query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDto.Index>> GetOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderDto.Android> GetOrderAsyncAndroid(string orderNumber)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalOrdersForSalesperson(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> GeneratePdf(string orderNumber)
    {
        var response = await httpClient.GetAsync($"order/{orderNumber}/pdf");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to generate PDF for quote {orderNumber}: {errorMessage}");
        }

        var pdfBytes = await response.Content.ReadAsByteArrayAsync();

        return pdfBytes;
    }
}
