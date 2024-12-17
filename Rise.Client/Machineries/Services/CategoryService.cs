using System.Net.Http.Json;
using Azure;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.Services;

public class CategoryService(HttpClient httpClient) : ICategoryService
{
    public async Task<CategoryDto.Detail> CreateCategoryAsync(CategoryDto.Create categoryDto)
    {
        var response = await httpClient.PostAsJsonAsync("category", categoryDto);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto.Detail>();
        return category!;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await httpClient.DeleteAsync($"category/{id}");
    }

    public async Task<IEnumerable<CategoryDto.Detail>> GetCategoriesAsync(CategoryQueryObject query)
    {
        string url = $"category?Search={query.Search}";
        var categories = await httpClient.GetFromJsonAsync<IEnumerable<CategoryDto.Detail>>(url);
        return categories!;
    }


    public async Task<CategoryDto.Detail> GetCategoryAsync(int id)
    {
        var category = await httpClient.GetFromJsonAsync<CategoryDto.Detail>($"category/{id}");
        return category!;
    }


    public async Task<CategoryDto.Detail> UpdateCategoryAsync(int id, CategoryDto.Update categoryDto)
    {
        var response = await httpClient.PutAsJsonAsync($"category/", categoryDto);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto.Detail>();
        return category!;
    }
}