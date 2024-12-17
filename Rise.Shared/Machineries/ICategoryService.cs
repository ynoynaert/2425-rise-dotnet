using Rise.Shared.Helpers;

namespace Rise.Shared.Machineries;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto.Detail>> GetCategoriesAsync(CategoryQueryObject query);
    Task<CategoryDto.Detail> GetCategoryAsync(int id);
    Task<CategoryDto.Detail> CreateCategoryAsync(CategoryDto.Create categoryDto);
    Task<CategoryDto.Detail> UpdateCategoryAsync(int id, CategoryDto.Update categoryDto);
    Task DeleteCategoryAsync(int id);
}

