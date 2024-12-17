using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.FakeServices;

public class FakeCategoryService : ICategoryService
{
    private List<CategoryDto.Detail> _allCategories;

    public FakeCategoryService()
    {
        _allCategories = Enumerable.Range(1, 5).Select(i => new CategoryDto.Detail
        {
            Id = i,
            Name = $"Category {i}",
            Code = $"Code {i}",
            Options = new List<OptionDto.Index>
            {
                new OptionDto.Index { Id = 1, Name = "Option 1", Code = "O1" },
                new OptionDto.Index { Id = 2, Name = "Option 2", Code = "02" },
                new OptionDto.Index { Id = 3, Name = "Option 3", Code = "03" }
            }
        }).ToList();
    }

    public Task<IEnumerable<CategoryDto.Detail>> GetCategoriesAsync(CategoryQueryObject query)
    {
        IEnumerable<CategoryDto.Detail> filteredCategories = _allCategories;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            filteredCategories = filteredCategories.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.Code.ToLower().Contains(search) ||
                c.Options.Any(o => o.Name.ToLower().Contains(search) || o.Code.ToLower().Contains(search)));
        }

        return Task.FromResult(filteredCategories);
    }


    public Task<CategoryDto.Detail> GetCategoryAsync(int id)
    {
        var category = _allCategories.FirstOrDefault(c => c.Id == id);
        if (category is null)
        {
            throw new Exception("Category not found");
        }
        return Task.FromResult(category);
    }

    public Task<CategoryDto.Detail> CreateCategoryAsync(CategoryDto.Create categoryDto)
    {
        var newId = _allCategories.Count + 1;
        var newCategory = new CategoryDto.Detail
        {
            Id = newId,
            Name = categoryDto.Name!,
            Code = categoryDto.Code!,
            Options = new List<OptionDto.Index>()
        };

        _allCategories.Add(newCategory);
        return Task.FromResult(newCategory);
    }

    public Task<CategoryDto.Detail> UpdateCategoryAsync(int id, CategoryDto.Update categoryDto)
    {
        var existingCategory = _allCategories.FirstOrDefault(c => c.Id == id);
        if (existingCategory != null)
        {
            existingCategory.Name = categoryDto.Name!;
            existingCategory.Code = categoryDto.Code!;
        }

        return Task.FromResult(existingCategory!);
    }

    public Task DeleteCategoryAsync(int id)
    {
        var categoryToRemove = _allCategories.FirstOrDefault(c => c.Id == id);
        if (categoryToRemove != null)
        {
            _allCategories.Remove(categoryToRemove);
        }
        return Task.CompletedTask;
    }
}
