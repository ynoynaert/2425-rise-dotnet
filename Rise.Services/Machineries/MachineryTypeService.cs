using Rise.Persistence;
using Rise.Shared.Machineries;
using Rise.Domain.Machineries;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Serilog;

namespace Rise.Services.Machineries;
public class MachineryTypeService(ApplicationDbContext dbContext) : IMachineryTypeService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<IEnumerable<MachineryTypeDto.Index>> GetMachineryTypesAsync()
    {
        Log.Information("MachineryTypes retrieved");
        return await dbContext.MachineryTypes
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryTypeDto.Index
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
    }

    public async Task<MachineryTypeDto.Index> GetMachineryTypeAsync(int id)
    {
        var type = await dbContext.MachineryTypes
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryTypeDto.Index
            {
                Id = x.Id,
                Name = x.Name
            })
            .SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machinetype", id);

        Log.Information("MachineryType retrieved by id");
        return new MachineryTypeDto.Index
        {
            Id = type.Id,
            Name = type.Name
        };
    }

    public async Task<MachineryTypeDto.Index> CreateMachineryTypeAsync(MachineryTypeDto.Create machineryTypeDto)
    {
        var existingType = await dbContext.MachineryTypes.SingleOrDefaultAsync(x => x.Name == machineryTypeDto.Name);

        if (existingType is not null)
        {
            Log.Warning("MachineryType already exists");
            throw new EntityAlreadyExistsException("Type", "Naam", machineryTypeDto.Name!);
        }

        var type = new MachineryType
        {
            Name = machineryTypeDto.Name!
        };
        dbContext.MachineryTypes.Add(type);
        await dbContext.SaveChangesAsync();
        Log.Information("MachineryType created");
        return new MachineryTypeDto.Index
        {
            Name = machineryTypeDto.Name!
        };
    }

    public async Task<MachineryTypeDto.Index> UpdateMachineryTypeAsync(int id, MachineryTypeDto.Update machineryTypeDto)
    {
        var machineryType = await dbContext.MachineryTypes.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machinetype", id);

        machineryType.Name = machineryTypeDto.Name!;

        var existingType = await dbContext.MachineryTypes
            .Where(x => x.Id != id)
            .SingleOrDefaultAsync(x => x.Name == machineryTypeDto.Name);

        if (existingType is not null)
        {
            Log.Warning("MachineryType already exists");
            throw new EntityAlreadyExistsException("Type", "Naam", machineryTypeDto.Name!);
        }

        await dbContext.SaveChangesAsync();
        Log.Information("MachineryType updated");

        return new MachineryTypeDto.Index
        {
            Id = machineryTypeDto.Id,
            Name = machineryTypeDto.Name!
        };
    }

    public async Task DeleteMachineryTypeAsync(int id)
    {
        var type = await dbContext.MachineryTypes.SingleAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machinetype", id);

        dbContext.MachineryTypes.Remove(type);
        await dbContext.SaveChangesAsync();
        Log.Information("MachineryType deleted");
    }
}
