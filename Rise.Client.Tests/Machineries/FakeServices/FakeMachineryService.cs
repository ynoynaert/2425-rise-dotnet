using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Domain.Machineries;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.FakeServices;

public class FakeMachineryService : IMachineryService
{
    private List<MachineryDto.Detail> _allMachineries;

    public FakeMachineryService()
    {
        _allMachineries = Enumerable.Range(1, 11)
            .Select(i => new MachineryDto.Detail
            {
                Id = i,
                Name = $"Machinery {i}",
                SerialNumber = $"SN{i}",
                Type = new MachineryTypeDto.Index { Id = i, Name = $"Type {i}" },
                Description = $"Description {i}",
                BrochureText = $"Brochure text {i}",
				Images = new List<ImageDto.Index>
				{
					new ImageDto.Index { Id = 1, Url = "https://via.placeholder.com/150" },
					new ImageDto.Index { Id = 2, Url = "https://via.placeholder.com/150" },
					new ImageDto.Index { Id = 3, Url = "https://via.placeholder.com/150" }
				},
				MachineryOptions = new List<MachineryOptionDto.Index>
                {
                    new MachineryOptionDto.Index { Id = 1, Price = 100, OptionId = 1 },
                    new MachineryOptionDto.Index { Id = 2, Price = 200, OptionId = 2 },
                    new MachineryOptionDto.Index { Id = 3, Price = 300, OptionId = 3 },
                }
            }).ToList();
    }

    public Task<IEnumerable<MachineryDto.Detail>> GetMachineriesAsync(MachineryQueryObject query)
    {
        var machineries = _allMachineries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            machineries = machineries.Where(x => x.Name.Contains(query.Search) || x.SerialNumber.Contains(query.Search));
        }

        if (!string.IsNullOrWhiteSpace(query.TypeIds))
        {
            var typeIds = query.TypeIds.Split("+").Select(int.Parse);
            machineries = machineries.Where(x => typeIds.Contains(x.Type.Id));
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            if (query.SortBy == "Name")
            {
                machineries = query.IsDescending ? machineries.OrderByDescending(x => x.Name) : machineries.OrderBy(x => x.Name);
            }
        }

        var skipNumber = (query.PageNumber - 1) * query.PageSize;
        var pagedMachineries = machineries.Skip(skipNumber).Take(query.PageSize);

        return Task.FromResult(pagedMachineries.AsEnumerable());
    }

    public Task<MachineryDto.Detail> GetMachineryAsync(int id)
    {
        var machinery = _allMachineries.FirstOrDefault(m => m.Id == id);
        if (machinery is null)
        {
            throw new Exception("Machinery not found");
        }
        return Task.FromResult(machinery!);
    }

    public Task<MachineryResult.Create> CreateMachineryAsync(MachineryDto.Create machinery)
    {
        var newId = _allMachineries.Count() + 1;
        var newMachinery = new MachineryDto.Detail
        {
            Id = newId,
            Name = machinery.Name!,
            SerialNumber = machinery.SerialNumber!,
            Type = new MachineryTypeDto.Index { Id = newId, Name = $"Type {newId}" },
            Description = machinery.Description!,
            BrochureText = machinery.BrochureText!,
            Images = new List<ImageDto.Index>
			{
				new ImageDto.Index { Id = 1, Url = "https://via.placeholder.com/150" },
				new ImageDto.Index { Id = 2, Url = "https://via.placeholder.com/150" },
				new ImageDto.Index { Id = 3, Url = "https://via.placeholder.com/150" }
			},
			MachineryOptions = new List<MachineryOptionDto.Index>
            {
                new MachineryOptionDto.Index { Id = 1, Price = 0, OptionId = 1 },
                new MachineryOptionDto.Index { Id = 2, Price = 0, OptionId = 2 },
                new MachineryOptionDto.Index { Id = 3, Price = 0, OptionId = 3 },
            }
        };

        _allMachineries.Add(newMachinery);
        return Task.FromResult(new MachineryResult.Create()
        {
            Id = newId,
            UploadUris = new List<string>
            {
                "https://via.placeholder.com/150",
                "https://via.placeholder.com/150",
                "https://via.placeholder.com/150"
            }
        });

    }

    public Task<MachineryResult.Create> UpdateMachineryAsync(int id, MachineryDto.Update machinery)
    {
        var existingMachinery = _allMachineries.FirstOrDefault(m => m.Id == id);
        if (existingMachinery != null)
        {
            existingMachinery.Name = machinery.Name!;
            existingMachinery.SerialNumber = machinery.SerialNumber!;
            existingMachinery.Description = machinery.Description!;
        }

        return Task.FromResult(new MachineryResult.Create
        {
            Id = id,
            UploadUris = new List<string>
            {
                "https://via.placeholder.com/150",
                "https://via.placeholder.com/150",
                "https://via.placeholder.com/150"
            }
        });
    }

    public Task DeleteMachineryAsync(int id)
    {
        var machineryToRemove = _allMachineries.FirstOrDefault(m => m.Id == id);
        if (machineryToRemove != null)
        {
            _allMachineries.Remove(machineryToRemove);
        }
        return Task.CompletedTask;
    }

    public Task<MachineryDto.XtremeDetail> GetMachineryAsyncWithCategories(int id)
    {
        var machinery = _allMachineries.FirstOrDefault(m => m.Id == id);
        if (machinery == null) return Task.FromResult<MachineryDto.XtremeDetail>(null!);

        var xtremeDetail = new MachineryDto.XtremeDetail
        {
            Id = machinery.Id,
            Name = machinery.Name,
            SerialNumber = machinery.SerialNumber,
            Type = machinery.Type,
            Description = machinery.Description,
            BrochureText = machinery.BrochureText,
            Images = new List<ImageDto.Index>
            {
                new ImageDto.Index { Id = 1, Url = "https://via.placeholder.com/150" },
                new ImageDto.Index { Id = 2, Url = "https://via.placeholder.com/150" },
                new ImageDto.Index { Id = 3, Url = "https://via.placeholder.com/150" }
            },
            MachineryOptions = new List<MachineryOptionDto.XtremeDetail>
            {
                new MachineryOptionDto.XtremeDetail
                {
                    Id = 1,
                    Price = 100,
                    Option = new OptionDto.Detail
                    {
                        Id = 1,
                        Name = "Option 1",
                        Code = "O1",
                        Category = new CategoryDto.Index
                        {
                            Id = 1,
                            Name = "Category 1",
                            Code = "C1"
                        }
                    }
                },
                new MachineryOptionDto.XtremeDetail
                {
                    Id = 2,
                    Price = 200,
                    Option = new OptionDto.Detail
                    {
                        Id = 2,
                        Name = "Option 2",
                        Code = "O2",
                        Category = new CategoryDto.Index
                        {
                            Id = 2,
                            Name = "Category 2",
                            Code = "C2"
                        }
                    }
                },
                new MachineryOptionDto.XtremeDetail
                {
                    Id = 3,
                    Price = 300,
                    Option = new OptionDto.Detail
                    {
                        Id = 3,
                        Name = "Option 3",
                        Code = "O3",
                        Category = new CategoryDto.Index
                        {
                            Id = 3,
                            Name = "Category 3",
                            Code = "C3"
                        }
                    }
                }
            }
        };

        return Task.FromResult(xtremeDetail);
    }

    public Task<int> GetTotalMachineriesAsync()
    {
        return Task.FromResult(_allMachineries.Count());
    }

    public Task<MachineryDto.Index> GetMachineryByMachineryNameAsync(string machineryName)
    {
        var machinery = _allMachineries.FirstOrDefault(m => m.Name == machineryName);
        if (machinery is null)
        {
            throw new Exception("Machinery not found");
        }
        var machineryIndex = new MachineryDto.Index
        {
            Id = machinery.Id,
            Name = machinery.Name,
            SerialNumber = machinery.SerialNumber,
            Type = machinery.Type,
            Description = machinery.Description
        };
        return Task.FromResult(machineryIndex);
    }


    public Task DeleteImageMachineryAsync(int id, int imageId)
    {
        throw new NotImplementedException();
    }

    public Task<MachineryDto.XtremeDetail> GetMachineryAsyncWithCategories(int id, OptionQueryObject query)
    {
        var machinery = _allMachineries.FirstOrDefault(m => m.Id == id);
        if (machinery == null)
        {
            throw new Exception($"Machinery with ID {id} not found.");
        }

        var xtremeDetail = new MachineryDto.XtremeDetail
        {
            Id = machinery.Id,
            Name = machinery.Name,
            SerialNumber = machinery.SerialNumber,
            Type = machinery.Type,
            Description = machinery.Description,
            BrochureText = machinery.BrochureText,
            Images = machinery.Images,
            MachineryOptions = machinery.MachineryOptions.Select(mo => new MachineryOptionDto.XtremeDetail
            {
                Id = mo.Id,
                Price = mo.Price,
                Option = new OptionDto.Detail
                {
                    Id = mo.OptionId,
                    Name = $"Option {mo.OptionId}",
                    Code = $"Code{mo.OptionId}",
                    Category = new CategoryDto.Index
                    {
                        Id = mo.OptionId,
                        Name = $"Category {mo.OptionId}",
                        Code = $"C{mo.OptionId}"
                    }
                }
            }).ToList()
        };

        if (!string.IsNullOrEmpty(query.Search))
        {
            xtremeDetail.MachineryOptions = xtremeDetail.MachineryOptions
                .Where(mo =>
                    mo.Option.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    mo.Option.Code.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    mo.Option.Category.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    mo.Option.Category.Code.Contains(query.Search, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
        }

        return Task.FromResult(xtremeDetail);
    }

}
