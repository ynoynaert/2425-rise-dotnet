using Microsoft.AspNetCore.Components;
using Rise.Domain.Quotes;
using Rise.Shared.Orders;
using Rise.Shared.Quotes;
using static Rise.Shared.Quotes.QuoteDto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.JSInterop;

namespace Rise.Client.Quotes;

public partial class QuoteDetails
{
    [Parameter]
    public string QuoteNumber { get; set; } = string.Empty;

    private QuoteDto.ExcelModel? ExcelModel;
    private bool isLoading = true;
    private string? errorLoading;
    private string? errorMessages;

    [Inject]
    public required IQuoteService QuoteService { get; set; }


    [Inject]
    public required IOrderService OrderService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private bool hasOrder;

    public bool IsTopTextVisible = false;
    public bool IsBottomTextVisible = false;

    protected override async Task OnInitializedAsync()
    {
        errorLoading = null;

        try
        {
            ExcelModel = await QuoteService.GetQuoteAsync(QuoteNumber);
            if (ExcelModel != null)
            {
                hasOrder = await OrderService.CheckIfQuoteHasOrder(QuoteNumber!.Split('-')[0]);
            }
        }
        catch (Exception ex)
        {

            errorLoading = $"Er is een fout opgetreden tijdens het ophalen van de offerte: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnApproveClicked() {
        errorMessages = null;

        IsTopTextVisible = false;
        IsBottomTextVisible = false;

        try
        {
            ExcelModel!.IsApproved = await QuoteService.ApproveQuote(QuoteNumber);

        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het goedkeuren van een offerte: " + ex.Message;

        }
    }
    private void OnButtonClicked()
    {
        NavigationManager.NavigateTo("/offertes");
    }
    private async Task OnMakeOrderClicked()
    {
        errorMessages = null;

        IsTopTextVisible = false;
        IsBottomTextVisible = false;

        try {
            var order = new OrderDto.Create
            {
                OrderNumber = ExcelModel!.QuoteNumber!.Split('-')[0],
                Date = DateTime.Now,
                QuoteId = ExcelModel!.Id
            };

            await OrderService.CreateOrderAsync(order);
            NavigationManager.NavigateTo($"/bestellingen/{order.OrderNumber}");
        } catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het maken van de order: " + ex.Message;

        }

    }
    private void OnAddOptionsButtonClicked()
    {
        NavigationManager.NavigateTo($"/offertes/{ExcelModel?.QuoteNumber}/opties");
    }

    private void OnSeeOrderClicked()
    {
        NavigationManager.NavigateTo($"/bestellingen/{QuoteNumber!.Split('-')[0]}");
    }

    private void OnAddTradedMachineryClicked() {
        NavigationManager.NavigateTo($"/offertes/{ExcelModel?.QuoteNumber}/ingeruilde-machine");
    }

    private async Task OnGeneratePdfClicked()
    {
        IsBottomTextVisible = false;
        IsTopTextVisible = false;
        try
        {
            var pdfBytes = await QuoteService.GeneratePdf(QuoteNumber);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new Exception("PDF generation failed: received empty byte array.");
            }

            var base64String = Convert.ToBase64String(pdfBytes);

            await JSRuntime.InvokeVoidAsync("downloadFileFromByteArray", $"offerte_{QuoteNumber}.pdf", base64String);
        }
        catch (Exception ex)
        {
            errorMessages = $"Er is een fout opgetreden tijdens het genereren van de PDF: {ex.Message}";
            Console.Error.WriteLine(ex);
        }
    }

    private void OnAddTextButtonClicked()
    {
        IsTopTextVisible = true;
        IsBottomTextVisible = true;
    }
}
