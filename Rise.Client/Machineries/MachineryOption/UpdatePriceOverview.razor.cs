using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using static Rise.Shared.Quotes.QuoteDto;

namespace Rise.Client.Machineries.MachineryOption;

public partial class UpdatePriceOverview
{
    [Parameter] public required List<MachineryOptionDto.Detail> PriceUpdateList { get; set; }

    [Inject]
    public IMachineryOptionService MachineryOptionService { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private int? updateToEditId { get; set; } = null;

    private void EditPriceUpdate(int Id)
    {
        updateToEditId = Id;
    }

    private void DeletePriceUpdate(int Id)
    {
        PriceUpdateList.RemoveAll(x => x.Id == Id);
    }

    private void SaveEdit(MachineryOptionDto.Detail updatedItem)
    {
        var item = PriceUpdateList.FirstOrDefault(x => x.Id == updatedItem.Id);
        if (item != null)
        {
            item.Price = updatedItem.Price;
        }

        updateToEditId = null;
    }

    private void CancelEdit()
    {
        updateToEditId = null;
    }
}
