using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Machineries;
using Blazorise.FluentValidation;
using Blazorise;

namespace Rise.Client.Machineries.Category;

public partial class AddCategory
{
    public CategoryDto.Create Model = new();

    [Inject]
    public required ICategoryService CategoryService { get; set; }
    private string? errorMessages;
    private Validations fluentValidations = new();

    async Task ValidateData(EditContext context)
    {
        errorMessages = null;

        try
        {
            if (fluentValidations.AllValidationsSuccessful)
            {
                await CategoryService.CreateCategoryAsync(Model);
                NavigationManager.NavigateTo($"/categorieën");
            }
            else 
            {
                errorMessages = "Gelieve alle verplichte velden in te vullen!";
            }
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het aanmaken van een categorie: " + ex.Message;
        }
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën");
    }

}
