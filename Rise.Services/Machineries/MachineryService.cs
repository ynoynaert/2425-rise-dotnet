using Microsoft.EntityFrameworkCore;
using Rise.Domain.Machineries;
using Rise.Persistence;
using Rise.Shared.Machineries;
using Rise.Domain.Exceptions;
using Rise.Shared.Helpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using Blazorise;
using Rise.Services.Files;
using Serilog;


namespace Rise.Services.Machineries;

public class MachineryService(IStorageService storageService, ApplicationDbContext dbContext) : IMachineryService
{
  

    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<IEnumerable<MachineryDto.Detail>> GetMachineriesAsync(MachineryQueryObject query)
    {
        IQueryable<MachineryDto.Detail> machineryQuery = dbContext.Machineries
        .Include(y => y.Type)
        .Include(y => y.MachineryOptions)
        .Where(x => !x.IsDeleted)
        .Select(x => new MachineryDto.Detail
        {
            Id = x.Id,
            Name = x.Name,
            SerialNumber = x.SerialNumber,
            Type = new MachineryTypeDto.Index
            {
                Id = x.Type.Id,
                Name = x.Type.Name,
            },
            Description = x.Description,
            BrochureText = x.BrochureText,
            Images = x.Images.Where(x => !x.IsDeleted).Select(i => new ImageDto.Index
            {
                Id = i.Id,
                Url = i.Url
            }).ToList(),
            MachineryOptions = x.MachineryOptions.Where(mo => !mo.IsDeleted)
                .Select(mo => new MachineryOptionDto.Index
                {
                    Id = mo.Id,
                    OptionId = mo.Option.Id,
                    MachineryId = mo.Machinery.Id,
                    Price = mo.Price
                }).ToList()
        });

        // search filter
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            machineryQuery = machineryQuery.Where(x => x.Name.Contains(query.Search) || x.SerialNumber.Contains(query.Search));
        }

        // type filter
        if (!string.IsNullOrWhiteSpace(query.TypeIds))
        {
            var typeIds = query.TypeIds.Split("-").Select(int.Parse).ToList();
            machineryQuery = machineryQuery.Where(x => typeIds.Contains(x.Type.Id));
        }

        // sortering
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            if (query.SortBy == "Name")
            {
                machineryQuery = query.IsDescending
                    ? machineryQuery.OrderByDescending(x => x.Name)
                    : machineryQuery.OrderBy(x => x.Name);
            }
            else if (query.SortBy == "Type")
            {
                machineryQuery = query.IsDescending
                    ? machineryQuery.OrderByDescending(x => x.Type.Name).ThenByDescending(x => x.Name)
                    : machineryQuery.OrderBy(x => x.Type.Name).ThenBy(x => x.Name);
            }
            else if (query.SortBy == "Serialnumber")
            {
                machineryQuery = query.IsDescending
                    ? machineryQuery.OrderByDescending(x => x.SerialNumber)
                    : machineryQuery.OrderBy(x => x.SerialNumber);
            }
        }
        else
        {
            machineryQuery = query.IsDescending
        ? machineryQuery.OrderByDescending(x => x.Type.Name).ThenByDescending(x => x.Name)
        : machineryQuery.OrderBy(x => x.Type.Name).ThenBy(x => x.Name);
        }

        var skipNumber = (query.PageNumber - 1) * query.PageSize;

        var machineries = await machineryQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();

        Log.Information("Machineries retrieved");
        return machineries;
    }

    public async Task<MachineryDto.Detail> GetMachineryAsync(int id)
    {
        var machinery = await dbContext.Machineries
            .Include(m => m.Type)
            .Include(m => m.MachineryOptions)
            .ThenInclude(mo => mo.Option)
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                SerialNumber = x.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = x.Type.Id,
                    Name = x.Type.Name,
                },
                Description = x.Description,
                BrochureText = x.BrochureText,
                Images = x.Images.Where(x => !x.IsDeleted).Select(i => new ImageDto.Index
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList(),
                MachineryOptions = x.MachineryOptions
                .Where(mo => !mo.IsDeleted)
                .Select(mo => new MachineryOptionDto.Index
                {
                    Id = mo.Id,
                    OptionId = mo.Option.Id,
                    MachineryId = mo.Machinery.Id,
                    Price = mo.Price
                }).ToList()
            }).SingleOrDefaultAsync(x => x.Id == id);

        if (machinery is null)
        {
            Log.Warning("Machinery not found by id");
            throw new EntityNotFoundException("Machine", id);
        }
        Log.Information("Machinery retrieved by id");
        return machinery;
    }

    public async Task<MachineryDto.Index> GetMachineryByMachineryNameAsync(string machineryName)
    {
        var machinery = await dbContext.Machineries
            .Include(m => m.Type)
            .Include(m => m.MachineryOptions)
            .ThenInclude(mo => mo.Option)
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryDto.Index
            {
                Id = x.Id,
                Name = x.Name,
                SerialNumber = x.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = x.Type.Id,
                    Name = x.Type.Name,
                },
                Description = x.Description,
            }).SingleOrDefaultAsync(x => x.Name == machineryName);

        if (machinery is null)
        {
            Log.Warning("Machinery not found by name");
            throw new EntityNotFoundException("Machine", machineryName);
        }

        Log.Information("Machinery retrieved by name");
        return machinery;
    }


    public async Task<MachineryResult.Create> CreateMachineryAsync(MachineryDto.Create machineryDto)
    {
        var existingMachinery = await dbContext.Machineries
                    .SingleOrDefaultAsync(m => m.SerialNumber == machineryDto.SerialNumber);

        if (existingMachinery is not null)
        {
            Log.Warning("Machinery already exists");
            throw new EntityAlreadyExistsException("Machine", "Serienummer", machineryDto.SerialNumber!);
        }

        var type = await dbContext.MachineryTypes.SingleOrDefaultAsync(x => x.Id == machineryDto.TypeId) ?? throw new EntityNotFoundException("Machinetype", machineryDto.TypeId);
        var machinery = new Machinery
        {
            Name = machineryDto.Name!,
            SerialNumber = machineryDto.SerialNumber!,
            Type = type,
            Description = machineryDto.Description!,
            BrochureText = machineryDto.BrochureText!,
        };

        dbContext.Machineries.Add(machinery);
        List<Uri> uris = new List<Uri>();
        foreach (var contentType in machineryDto!.ImageContentType)
        {
            Domain.Files.Image image = new Domain.Files.Image(storageService.BasePath, contentType);
            Uri uploadSas = storageService.GenerateImageUploadSas(image);
            uris.Add(uploadSas);
            Domain.Machineries.Image machineryImage = new Domain.Machineries.Image { Machinery = machinery, Url = image.FileUri.ToString() };
            dbContext.Images.Add(machineryImage);
        }

        await dbContext.SaveChangesAsync();
        Log.Information("Machinery created");

        return new MachineryResult.Create()
        {
            Id = machinery.Id,
            UploadUris = uris.Select(x => x.ToString()).ToList()
        };
    }


    public async Task<MachineryResult.Create> UpdateMachineryAsync(int id, MachineryDto.Update machineryDto)
    {

        var machinery = await dbContext.Machineries
                        .Include(m => m.MachineryOptions.Where(mo => !mo.IsDeleted))
                        .ThenInclude(mo => mo.Option)
                        .SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machine", id);

        if (machineryDto is null)
        {
            Log.Warning("Machinery not found by id");
            throw new EntityNotFoundException("MachineDto", id);
        }

        var existingMachinery = await dbContext.Machineries
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.Id != machineryDto.Id)
                    .SingleOrDefaultAsync(m => m.SerialNumber == machineryDto.SerialNumber);

        if (existingMachinery is not null)
        {
			Log.Warning("Machinery already exists");
			throw new EntityAlreadyExistsException("Machine", "Serienummer", machineryDto.SerialNumber!);
		}

		var type = await dbContext.MachineryTypes.SingleOrDefaultAsync(x => x.Id == machineryDto.TypeId) ?? throw new EntityNotFoundException("Machinetype", machineryDto.TypeId);
        machinery.Name = machineryDto.Name!;
        machinery.SerialNumber = machineryDto.SerialNumber!;
        machinery.Type = type;
        machinery.Description = machineryDto.Description!;
        machinery.BrochureText = machineryDto.BrochureText!;

        List<Uri> uris = new List<Uri>();
        foreach (var contentType in machineryDto!.ImageContentTypeNew)
        {
            Domain.Files.Image image = new Domain.Files.Image(storageService.BasePath, contentType);
            Uri uploadSas = storageService.GenerateImageUploadSas(image);
            uris.Add(uploadSas);
            Domain.Machineries.Image machineryImage = new Domain.Machineries.Image { Machinery = machinery, Url = image.FileUri.ToString() };
            dbContext.Images.Add(machineryImage);
        }

        await dbContext.SaveChangesAsync();
        Log.Information("Machinery updated");

        return new MachineryResult.Create()
        {
            Id = machinery.Id,
            UploadUris = uris.Select(x => x.ToString()).ToList()
        };
    }

    public async Task DeleteMachineryAsync(int id)
    {
        var machinery = await dbContext.Machineries.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machine", id);
        var machineryOptions = await dbContext.MachineryOptions.Where(x => x.Machinery.Id == id).ToListAsync();
        var images = await dbContext.Images.Where(x => x.Machinery.Id == id).ToListAsync();
        dbContext.Machineries.Remove(machinery);
        dbContext.MachineryOptions.RemoveRange(machineryOptions);
        dbContext.Images.RemoveRange(images);

        await dbContext.SaveChangesAsync();
        Log.Information("Machinery deleted");
    }

    public Task<int> GetTotalMachineriesAsync()
    {
        Log.Information("Total machineries retrieved");
        return dbContext.Machineries.CountAsync();
    }

    public async Task<MachineryDto.XtremeDetail> GetMachineryAsyncWithCategories(int id, OptionQueryObject query)
    {
        // Fetch machinery with related data
        var machinery = await dbContext.Machineries
            .Include(m => m.Type)
            .Include(m => m.MachineryOptions)
            .ThenInclude(mo => mo.Option)
            .ThenInclude(o => o.Category)
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryDto.XtremeDetail
            {
                Id = x.Id,
                Name = x.Name,
                SerialNumber = x.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = x.Type.Id,
                    Name = x.Type.Name,
                },
                Description = x.Description,
                BrochureText = x.BrochureText,
                Images = x.Images.Where(x => !x.IsDeleted).Select(i => new ImageDto.Index
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList(),
                MachineryOptions = x.MachineryOptions
                    .Where(mo => !mo.IsDeleted)
                    .Select(mo => new MachineryOptionDto.XtremeDetail
                    {
                        Id = mo.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = mo.Option.Id,
                            Name = mo.Option.Name,
                            Code = mo.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = mo.Option.Category.Id,
                                Name = mo.Option.Category.Name,
                                Code = mo.Option.Category.Code
                            }
                        },
                        Price = mo.Price
                    }).ToList()
            })
            .SingleOrDefaultAsync(x => x.Id == id);

        // If no machinery is found, throw an exception
        if (machinery is null)
        {
            Log.Warning("Machinery not found by id");
            throw new EntityNotFoundException("Machine", id);
        }

        // Apply search filter on the in-memory list
        if (!string.IsNullOrEmpty(query.Search))
        {
            machinery.MachineryOptions = machinery.MachineryOptions
                .Where(mo =>
                    mo.Option.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase) || // Search in Option Name
                    mo.Option.Code.Contains(query.Search, StringComparison.OrdinalIgnoreCase) || // Search in Option Code
                    mo.Option.Category.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase) || // Search in Category Name
                    mo.Option.Category.Code.Contains(query.Search, StringComparison.OrdinalIgnoreCase) // Search in Category Code
                )
                .ToList();
        }

        Log.Information("Machinery retrieved by id with categories");
        return machinery;
    }



    public async Task DeleteImageMachineryAsync(int id, int imageId)
    {
        var machinery = await dbContext.Machineries.SingleAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Machine", id);
        var image = await dbContext.Images.Where(x => x.Machinery.Id == id).Where(x => x.Id == imageId).SingleAsync();
        dbContext.Images.Remove(image);

        await dbContext.SaveChangesAsync();
        Log.Information("Image deleted");
    }
}