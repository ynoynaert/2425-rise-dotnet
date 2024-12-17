using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Orders;
using Rise.Shared.Quotes;

namespace Rise.Client.Orders;

public partial class OrderDetails
{
    [Parameter]
    public String OrderNumber { get; set; } = string.Empty;

    private OrderDto.Detail? Order;
    private QuoteDto.ExcelModel? QuoteModel;
    private bool isLoading = true;
    private string? errorLoading;
    private string? errorMessages;

    [Inject]
    public required IOrderService OrderService { get; set; }

    [Inject]
    public required IQuoteService QuoteService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        errorLoading = null;
        try
        {
            Order = await OrderService.GetOrderAsync(OrderNumber);
            QuoteModel = Order?.Quote;
        }
        catch (Exception ex)
        {
            errorLoading = "Er is een fout opgetreden tijdens het ophalen van de bestelling: " +ex.Message ;
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/bestellingen");
    }

    private async Task OnCancelClicked()
    {
        errorMessages = null;
        try
        {
            var update = new OrderDto.Update
            {
                IsCancelled = true,
                OrderNumber = Order!.OrderNumber,
                Date = Order!.Date,
                Quote = Order!.Quote

            };

            Order = await OrderService.UpdateOrderAsync(Order.OrderNumber, update);
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het annuleren van de bestelling: " + ex.Message;
        }
    }

    private async Task OnGeneratePdfClicked()
    {
        try
        {
            var pdfBytes = await OrderService.GeneratePdf(OrderNumber);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new Exception("PDF generation failed: received empty byte array.");
            }

            var base64String = Convert.ToBase64String(pdfBytes);

            await JSRuntime.InvokeVoidAsync("downloadFileFromByteArray", $"bestelling_{OrderNumber}.pdf", base64String);
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het genereren van de PDF: {ex.Message}";
            Console.Error.WriteLine(ex);
        }
    }

}
