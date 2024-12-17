using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.Category;

public partial class EditCategory
{
    [Parameter]
    public int Id { get; set; }

    public required CategoryDto.Update Model;

    [Inject]
    public required ICategoryService CategoryService { get; set; }

    private string? errorMessages;
    private Validations fluentValidations = new();

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            var category = await CategoryService.GetCategoryAsync(Id);
            Model = new CategoryDto.Update
            {
                Id = category.Id,
                Name = category.Name!,
                Code = category.Code!,
            };
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het het ophalen van de data: " + ex.Message;
        }
    }

    async Task ValidateData(EditContext context)
    {
        errorMessages = null;
        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await CategoryService.UpdateCategoryAsync(Id, Model);
                NavigationManager.NavigateTo($"/categorieën/{Id}");
            }
            else
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Error bij het bewerken van een categorie: " + ex.Message;
        }
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}");
    }
}
