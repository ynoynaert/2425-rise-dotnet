using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Domain.Machineries;
using Rise.Shared.Machineries;
using Rise.Domain.Exceptions;
using Rise.Persistence.Migrations;
using Serilog;

namespace Rise.Services.Machineries;

public class OptionService(ApplicationDbContext dbContext) : IOptionService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<IEnumerable<OptionDto.Detail>> GetOptionsAsync()
    {
        IQueryable<OptionDto.Detail> query = dbContext.Options
            .Where(x => !x.IsDeleted)
            .Select(x => new OptionDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Category = new CategoryDto.Index
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    Code = x.Category.Code
                }
            });

        var options = await query.ToListAsync();
        Log.Information("Options retrieved");

        return options;
    }

    public async Task<OptionDto.Detail> GetOptionAsync(int id)
    {
        var option = await dbContext.Options
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category)
            .Select(x => new OptionDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Category = new CategoryDto.Index
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    Code = x.Category.Code
                }
            }).SingleOrDefaultAsync(x => x.Id == id);
        Log.Information("Option retrieved by id");

        return option ?? throw new EntityNotFoundException("Optie", id);
    }

    public async Task<OptionDto.Detail> CreateOptionAsync(OptionDto.Create optionDto)
    {
        var existingOption = await dbContext.Options
            .SingleOrDefaultAsync(m => m.Code == optionDto.Code);

        if (existingOption is not null)
        {
            Log.Warning("Option already exists");
            throw new EntityAlreadyExistsException("Optie", "Code", optionDto.Code!);
        }

        var category = await dbContext.Categories.SingleAsync(x => x.Id == optionDto.CategoryId);
        var option = new Option
        {
            Name = optionDto.Name!,
            Code = optionDto.Code!,
            Category = category
        };

        dbContext.Options.Add(option);
        await dbContext.SaveChangesAsync();
        Log.Information("Option created");

        return new OptionDto.Detail
        {
            Id = option.Id,
            Name = option.Name,
            Code = option.Code,
            Category = new CategoryDto.Index
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code
            }
        };
    }

    public async Task<OptionDto.Detail> UpdateOptionAsync(int id, OptionDto.Update optionDto)
    {
        var option = await dbContext.Options.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Optie", id); ;

        option.Name = optionDto.Name;
        option.Code = optionDto.Code!;
        var category = await dbContext.Categories.SingleAsync(x => x.Id == optionDto.CategoryId);

        var existingOption = await dbContext.Options
            .Where(x => x.Id != id)
            .SingleOrDefaultAsync(m => m.Code == optionDto.Code);

        if (existingOption is not null)
        {
            Log.Warning("Option already exists");
            throw new EntityAlreadyExistsException("Option", "Code", optionDto.Code!);
        }

        await dbContext.SaveChangesAsync();
        Log.Information("Option updated");

        return new OptionDto.Detail
        {
            Id = option.Id,
            Name = option.Name,
            Code = option.Code,
            Category = new CategoryDto.Index
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code
            }
        };
    }

    public async Task DeleteOptionAsync(int id)
    {
        var option = await dbContext.Options.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Optie", id);

        dbContext.Options.Remove(option);
        await dbContext.SaveChangesAsync();
        Log.Information("Option deleted");
    }
}

