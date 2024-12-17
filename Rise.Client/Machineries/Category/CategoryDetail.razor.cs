using Microsoft.AspNetCore.Components;
using Rise.Shared.Machineries;
using Rise.Client.Machineries.Components;

namespace Rise.Client.Machineries.Category;

partial class CategoryDetail
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    public required ICategoryService CategoryService { get; set; }

    [Inject]
    public required IOptionService OptionService { get; set; }

    private CategoryDto.Detail? categoryDetail;

    private string? errorMessages;

    private ConfirmationModal? confirmationModal;

    private int optionToDelete;
    private int deleteCategoryId;
    private int? optionToEditId { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        errorMessages = null;
        try
        {
            categoryDetail = await CategoryService.GetCategoryAsync(Id);
        }
        catch (Exception ex)
        {
            errorMessages = "Er is een fout opgetreden tijdens het ophalen van de categorie: " + ex.Message;
        }
    }

    private void EditOption(int optionId)
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}/bewerken/{optionId}");
    }

    private void EditCategory(int Id)
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}/bewerken");
    }

    private async Task ShowDeleteOptionConfirmation(OptionDto.Index option)
    {
        optionToDelete = option.Id;
        deleteCategoryId = 0;
        await confirmationModal!.ShowAsync($"Ben je zeker dat je optie {option.Name} wil verwijderen?");
    }

    private async Task OnDeleteConfirmed(bool isConfirmed)
    {
        if (isConfirmed)
        {
            if (optionToDelete != 0)
            {
                await DeleteOption(optionToDelete);
                optionToDelete = 0;
            }
            else if (deleteCategoryId != 0)
            {
                await OnDeleteCategoryButtonClicked(deleteCategoryId);
                deleteCategoryId = 0;
            }
        }
    }

    private async Task DeleteOption(int optionId)
    {
        await OptionService.DeleteOptionAsync(optionId);
        categoryDetail = await CategoryService.GetCategoryAsync(Id);
    }

    private async Task ShowDeleteCategoryConfirmation(CategoryDto.Detail category)
    {
        deleteCategoryId = category.Id;
        optionToDelete = 0;
        await confirmationModal!.ShowAsync($"Ben je zeker dat je categorie {category.Name} wil verwijderen?");
    }

    private async Task OnDeleteCategoryButtonClicked(int Id)
    {
        await CategoryService.DeleteCategoryAsync(Id);
        NavigationManager.NavigateTo($"/categorieën");
    }

    private void AddOption()
    {
        NavigationManager.NavigateTo($"/categorieën/{Id}/toevoegen");
    }

    void OnButtonClicked()
    {
        NavigationManager.NavigateTo($"/categorieën");
    }
}
