using Microsoft.AspNetCore.Components;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public partial class TradedMachineryRow
{
    [Parameter] public required TradedMachineryDto.Index Trade { get; set; }

    private string? SelectedImage;

    private void SelectImage(string image)
    {
        SelectedImage = image;
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        SelectedImage = Trade.Images.FirstOrDefault()?.Url;
    }
}