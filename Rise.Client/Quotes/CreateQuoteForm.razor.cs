using Blazorise.Extensions;
using ClosedXML.Report.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using System.Reflection.PortableExecutable;

namespace Rise.Client.Quotes;

public partial class CreateQuoteForm
{
    [Parameter] public required QuoteDto.ExcelModel ExcelModel { get; set; }
    [Parameter] public bool IsTopTextVisible { get; set; }
    [Parameter] public bool IsBottomTextVisible { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IQuoteService QuoteService { get; set; }

    public required QuoteDto.Update Model;
    private string? errorMessages;

    private List<IGrouping<string, QuoteOptionDto.OptionInfo>> options = new();

    override protected void OnInitialized()
    {
        options = ExcelModel.QuoteOptions.GroupBy(op => op.MachineryOption.Option.Category.Name).ToList();

        Model = new QuoteDto.Update
        {
            quoteNumber = ExcelModel.QuoteNumber!,
            TopText = ExcelModel.TopText,
            BottomText = ExcelModel.BottomText
        };
    }

    private void CloseTopTextEditor()
    {
        IsTopTextVisible = false;
    }
    private void CloseBottomTextEditor()
    {
        IsBottomTextVisible = false;
    }
    private void ValidateTopText()
    {
        errorMessages = null;
        try
        {
            if (Model.TopText.IsNullOrWhiteSpace())
            {
                Model.TopText = null;
            }
            QuoteService.UpdateQuoteAsync(ExcelModel.QuoteNumber!, Model);
            ExcelModel.TopText = Model.TopText;
            IsTopTextVisible = false;
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het bewerken van de tekst: " + ex.Message;
        }
    }
    private void ValidateBottomText()
    {
        errorMessages = null;
        try
        {
            if(Model.BottomText.IsNullOrWhiteSpace())
            {
                Model.BottomText = null;
            }
            QuoteService.UpdateQuoteAsync(ExcelModel.QuoteNumber!, Model);
            ExcelModel.BottomText = Model.BottomText;
            IsBottomTextVisible = false;
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het bewerken van de tekst: " + ex.Message;
        }
    }
}
