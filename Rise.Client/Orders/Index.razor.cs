using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Orders;
using Rise.Shared.Quotes;

namespace Rise.Client.Orders;
public partial class Index
{
    private IEnumerable<OrderDto.Index>? orders;
    private string? errorMessages;

    [Inject]
    public required IOrderService OrderService { get; set; }
    [Inject] public required IQuoteService QuoteService { get; set; }
    private OrderQueryObject query = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            orders = await OrderService.GetOrdersAsync(query);
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden bij het laden van de bestellingen: {ex.Message}";
        }
    }
}