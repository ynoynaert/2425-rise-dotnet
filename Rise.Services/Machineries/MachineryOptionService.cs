using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Domain.Machineries;
using Rise.Shared.Machineries;
using Rise.Domain.Exceptions;
using Rise.Persistence.Migrations;
using Rise.Domain.Quotes;
using ClosedXML.Excel;
using Serilog;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Rise.Services.Machineries;

public class MachineryOptionService(ApplicationDbContext dbContext) : IMachineryOptionService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<IEnumerable<MachineryOptionDto.Detail>> GetMachineryOptionsAsync()
    {
        IQueryable<MachineryOptionDto.Detail> query = dbContext.MachineryOptions
            .Where(x => !x.IsDeleted)
            .Include(x => x.Machinery)
            .Select(x => new MachineryOptionDto.Detail
            {
                Id = x.Id,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Machinery.Id,
                    Name = x.Machinery.Name,
                    SerialNumber = x.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Machinery.Type.Id,
                        Name = x.Machinery.Type.Name,
                    },
                    Description = x.Machinery.Description
                },
                Option = new OptionDto.Index
                {
                    Id = x.Option.Id,
                    Name = x.Option.Name,
                    Code = x.Option.Code
                },
                Price = x.Price,
            }
        );
        var machineryOptions = await query.ToListAsync();
        Log.Information("MachineryOptions retrieved");
        return machineryOptions;

    }

    public async Task<MachineryOptionDto.Detail> GetMachineryOptionAsync(int id)
    {
        var machineryOption = await dbContext.MachineryOptions
            .Where(x => !x.IsDeleted)
            .Include(x => x.Machinery)
            .Select(x => new MachineryOptionDto.Detail
            {
                Id = x.Id,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Machinery.Id,
                    Name = x.Machinery.Name,
                    SerialNumber = x.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Machinery.Type.Id,
                        Name = x.Machinery.Type.Name,
                    },
                    Description = x.Machinery.Description
                },
                Option = new OptionDto.Index
                {
                    Id = x.Option.Id,
                    Name = x.Option.Name,
                    Code = x.Option.Code
                },
                Price = x.Price
            }).SingleOrDefaultAsync(x => x.Id == id);

        if (machineryOption is null)
        {
            Log.Warning("MachineryOption not found by id");
            throw new EntityNotFoundException("Machineoptie", id);
        }
        Log.Information("MachineryOption retrieved by id");
        return machineryOption;
    }

    public async Task<MachineryOptionDto.Detail> CreateMachineryOptionAsync(MachineryOptionDto.Create machineryOptionDto)
    {
        var machinery = await dbContext.Machineries.Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == machineryOptionDto.MachineryId) ?? throw new EntityNotFoundException("Machine", machineryOptionDto.MachineryId);
        var option = await dbContext.Options.SingleOrDefaultAsync(x => x.Id == machineryOptionDto.OptionId) ?? throw new EntityNotFoundException("Optie", machineryOptionDto.OptionId);

        var machineryOption = new MachineryOption
        {
            Machinery = machinery,
            Option = option,
            Price = machineryOptionDto.Price,
        };

        dbContext.MachineryOptions.Add(machineryOption);
        await dbContext.SaveChangesAsync();
        Log.Information("MachineryOption created");
        return new MachineryOptionDto.Detail
        {
            Id = machineryOption.Id,
            Machinery = new MachineryDto.Index
            {
                Id = machineryOption.Machinery.Id,
                Name = machineryOption.Machinery.Name,
                SerialNumber = machineryOption.Machinery.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = machineryOption.Machinery.Type.Id,
                    Name = machineryOption.Machinery.Type.Name,
                },
                Description = machineryOption.Machinery.Description
            },
            Option = new OptionDto.Index
            {
                Id = machineryOption.Option.Id,
                Name = machineryOption.Option.Name,
                Code = machineryOption.Option.Code
            },
            Price = machineryOption.Price,
        };
    }

    public async Task<MachineryOptionDto.Detail> UpdateMachineryOptionAsync(int id, MachineryOptionDto.Update machineryOptionDto)
    {

        var machineryOption = await dbContext.MachineryOptions.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machineoptie", id);
        var machinery = await dbContext.Machineries.Include(x => x.Type).Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == machineryOptionDto.MachineryId) ?? throw new EntityNotFoundException("Machine", machineryOptionDto.MachineryId);
        var option = await dbContext.Options.Where(x => !x.IsDeleted).SingleOrDefaultAsync(x => x.Id == machineryOptionDto.OptionId) ?? throw new EntityNotFoundException("Optie", machineryOptionDto.OptionId);

        machineryOption.Machinery = machinery;
        machineryOption.Option = option;
        machineryOption.Price = machineryOptionDto.Price;

        await dbContext.SaveChangesAsync();
        Log.Information("MachineryOption updated");
        return new MachineryOptionDto.Detail
        {
            Id = machineryOption.Id,
            Machinery = new MachineryDto.Index
            {
                Id = machineryOption.Machinery.Id,
                Name = machineryOption.Machinery.Name,
                SerialNumber = machineryOption.Machinery.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = machineryOption.Machinery.Type.Id,
                    Name = machineryOption.Machinery.Type.Name,
                },
                Description = machineryOption.Machinery.Description
            },
            Option = new OptionDto.Index
            {
                Id = machineryOption.Option.Id,
                Name = machineryOption.Option.Name,
                Code = machineryOption.Option.Code
            },
            Price = machineryOption.Price,
        };
    }

    public async Task DeleteMachineryOptionAsync(int id)
    {
        var machineryOption = await dbContext.MachineryOptions.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machineoptie", id);

        dbContext.MachineryOptions.Remove(machineryOption);
        await dbContext.SaveChangesAsync();
        Log.Information("MachineryOption deleted");
    }

    public Task<List<MachineryOptionDto.Detail>> ImportPriceUpdateFile(string fileBase64)
    {
        var machineryOptionsList = new List<MachineryOptionDto.Detail>();

        var fileBytes = Convert.FromBase64String(fileBase64);
        using var memoryStream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);

        for (int row = 1; row <= worksheet.LastRowUsed()?.RowNumber(); row++)
        {
            var optionCode = worksheet.Cell(row, 1).GetValue<string>();
            var machinerySerialNumber = worksheet.Cell(row, 2).GetValue<string>();
            var newPrice = worksheet.Cell(row, 3).GetValue<string>();

            var mo = dbContext.MachineryOptions
                .Include(x => x.Machinery)
                .Include(x => x.Machinery.Type)
                .Include(x => x.Option)
                .SingleOrDefault(x => x.Machinery.SerialNumber == machinerySerialNumber && x.Option.Code == optionCode) ?? throw new EntityNotFoundException("Optie bij machine", $"{optionCode} bij {machinerySerialNumber}");

            if (!decimal.TryParse(newPrice, out var price))
            {
                continue;
            }

            var machineryOptionDetail = new MachineryOptionDto.Detail
            {
                Id = mo.Id,
                Machinery = new MachineryDto.Index
                {
                    Id = mo.Machinery.Id,
                    Name = mo.Machinery.Name,
                    SerialNumber = machinerySerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = mo.Machinery.Type.Id,
                        Name = mo.Machinery.Type.Name,
                    },
                    Description = mo.Machinery.Description
                },
                Option = new OptionDto.Index
                {
                    Id = mo.Option.Id,
                    Name = mo.Option.Name,
                    Code = optionCode
                },
                Price = price,
            };

            machineryOptionsList.Add(machineryOptionDetail);

        }
        Log.Information("MachineryOption price updated");
        return Task.FromResult(machineryOptionsList);
    }
}