using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Domain.Machineries;
using Rise.Shared.Machineries;
using Rise.Domain.Exceptions;
using Rise.Persistence.Migrations;
using Rise.Shared.Helpers;
using Serilog;

namespace Rise.Services.Machineries;

public class CategoryService(ApplicationDbContext dbContext) : ICategoryService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<IEnumerable<CategoryDto.Detail>> GetCategoriesAsync(CategoryQueryObject queryObject)
    {
        IQueryable<Category> query = dbContext.Categories
            .Include(x => x.Options)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(queryObject.Search))
        {
            var searchTerm = queryObject.Search.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.Code.ToLower().Contains(searchTerm) ||
                x.Options.Any(y =>
                    y.Name.ToLower().Contains(searchTerm) ||
                    y.Code.ToLower().Contains(searchTerm)
                )
            );
        }

        var categories = await query.Select(x => new CategoryDto.Detail
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Options = x.Options
                .Where(y => !y.IsDeleted)
                .Select(y => new OptionDto.Index
                {
                    Id = y.Id,
                    Name = y.Name,
                    Code = y.Code
                }).ToList()
        }).ToListAsync();

        Log.Information("Categories retrieved");
        return categories;
    }



    public async Task<CategoryDto.Detail> GetCategoryAsync(int id)
    {
        var category = await dbContext.Categories
            .Where(x => !x.IsDeleted)
            .Include(x => x.Options)
            .Select(x => new CategoryDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Options = x.Options
            .Where(x => !x.IsDeleted)
            .Select(x => new OptionDto.Index
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).ToList()
            }).SingleOrDefaultAsync(x => x.Id == id);

        Log.Information("Category retrieved by id");
        return category ?? throw new EntityNotFoundException("Categorie", id);
    }

    public async Task<CategoryDto.Detail> CreateCategoryAsync(CategoryDto.Create categoryDto)
    {
        var existingCategory = await dbContext.Categories
            .SingleOrDefaultAsync(m => m.Code == categoryDto.Code);

        if (existingCategory is not null)
        {
            throw new EntityAlreadyExistsException("Categorie", "Code", categoryDto.Code!);
        }

        var category = new Category
        {
            Name = categoryDto.Name!,
            Code = categoryDto.Code!
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        Log.Information("Category created");
        return new CategoryDto.Detail
        {
            Name = category.Name,
            Code = category.Code,
            Options = [],
        };
    }

    public async Task<CategoryDto.Detail> UpdateCategoryAsync(int id, CategoryDto.Update categoryDto)
    {
        var category = await dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Categorie", id);

        category.Name = categoryDto.Name;
        category.Code = categoryDto.Code!;

        var existingCategory = await dbContext.Categories
            .Where(x => !x.IsDeleted)
            .Where(x => x.Id != id)
            .SingleOrDefaultAsync(m => m.Code == categoryDto.Code);

        if (existingCategory is not null)
        {
            throw new EntityAlreadyExistsException("Categorie", "Code", categoryDto.Code!);
        }

        await dbContext.SaveChangesAsync();
        Log.Information("Category updated");
        return new CategoryDto.Detail
        {
            Name = category.Name,
            Code = category.Code,
            Options = category.Options.Select(x => new OptionDto.Index
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code
            }).ToList()
        }; ;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Categorie", id);

        var optionsToRemove = category.Options.ToList();

        foreach (var mo in optionsToRemove)
        {
            Log.Information("Option removed");
            category.RemoveOption(mo);
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
        Log.Information("Category deleted");
    }

}



