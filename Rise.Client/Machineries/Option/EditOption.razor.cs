using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.Option;

partial class EditOption
{
    [Parameter]
    public int Id { get; set; }

    [Parameter]
    public int OptionId { get; set; }

    [Inject]
    public required ICategoryService CategoryService { get; set; }

    [Inject]
    public required IOptionService OptionService { get; set; }

    private CategoryDto.Detail? categoryDetail;

    public required OptionDto.Update Model;

    private string? errorMessages;
    private Validations fluentValidations = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            categoryDetail = await CategoryService.GetCategoryAsync(Id);
            var option = await OptionService.GetOptionAsync(OptionId);
            Model = new OptionDto.Update
            {
                Id = option.Id,
                Name = option.Name!,
                Code = option.Code!,
                CategoryId = option.Category.Id,
            };
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het het ophalen van de data: " + ex.Message;
        }
    }

    private async Task ValidateData(EditContext context)
    {
        errorMessages = null;

        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await OptionService.UpdateOptionAsync(OptionId, Model);
                NavigationManager.NavigateTo($"/categorieën/{Id}");
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het bewerken van een optie: " + ex.Message;
        }
    }

    private void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}");
    }
}
