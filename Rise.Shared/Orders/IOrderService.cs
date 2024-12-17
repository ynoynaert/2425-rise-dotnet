using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;

namespace Rise.Shared.Orders;

public interface IOrderService
{
    Task<IEnumerable<OrderDto.Index>> GetOrdersForSalespersonAsync(string userId, OrderQueryObject query);
    Task<IEnumerable<OrderDto.Index>> GetOrdersAsync(OrderQueryObject query);
    Task<OrderDto.Index> CreateOrderAsync(OrderDto.Create orderDto);
    Task<OrderDto.Detail> GetOrderForSalespersonAsync(string userId, string orderNumber);
    Task<OrderDto.Detail> GetOrderAsync(string orderNumber);

    Task<bool> CheckIfQuoteHasOrder(string orderNumber);
    Task<int> GetTotalOrders();
    Task<int> GetTotalOrdersForSalesperson(string userId);
    Task<OrderDto.Detail> UpdateOrderAsync(string orderNumber, OrderDto.Update machineryDto);

    Task<OrderDto.Android> GetOrderForSalespersonAsyncAndroid(string userId, string orderNumber);
    Task<byte[]> GeneratePdf(string orderNumber);
    Task<OrderDto.Android> GetOrderAsyncAndroid(string orderNumber);
}
