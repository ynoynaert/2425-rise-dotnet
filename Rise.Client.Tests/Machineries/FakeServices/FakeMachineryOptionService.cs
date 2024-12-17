using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.FakeServices;

public class FakeMachineryOptionService : IMachineryOptionService
{
    // Interne lijst om machinopties bij te houden
    private readonly List<MachineryOptionDto.Detail> _machineryOptions;

    public FakeMachineryOptionService()
    {
        _machineryOptions = Enumerable.Range(1, 3)
            .Select(i => new MachineryOptionDto.Detail
            {
                Id = i,
                Price = 1000.00m + i,
                Machinery = new MachineryDto.Index { Id = i, Name = $"Machinery {i}", SerialNumber = $"SerialNumber {i}", Type = new MachineryTypeDto.Index { Id = i, Name = $"Type {i}" }, Description = $"Description {i}" },
                Option = new OptionDto.Index { Id = i, Name = $"Option {i}", Code = $"O{i}" }
            })
            .ToList();
    }

    public Task<MachineryOptionDto.Detail> CreateMachineryOptionAsync(MachineryOptionDto.Create machineryOptionDto)
    {

		var newOption = new MachineryOptionDto.Detail
        {
            Id = _machineryOptions.Count + 1, // Genereer een nieuw Id
            Price = machineryOptionDto.Price,
            Machinery = new MachineryDto.Index
            {
                Id = machineryOptionDto.MachineryId,
                Name = $"Machinery {machineryOptionDto.MachineryId}",
                SerialNumber = $"SerialNumber {machineryOptionDto.MachineryId}",
                Type = new MachineryTypeDto.Index { Id = machineryOptionDto.MachineryId, Name = $"Type {machineryOptionDto.MachineryId}" },
                Description = $"Description {machineryOptionDto.MachineryId}"
            },
            Option = new OptionDto.Index
            {
                Id = machineryOptionDto.OptionId,
                Name = $"Option {machineryOptionDto.OptionId}",
                Code = $"O{machineryOptionDto.OptionId}"
            }
        };

        _machineryOptions.Add(newOption); // Voeg toe aan de lijst
        return Task.FromResult(newOption);
    }

    public Task DeleteMachineryOptionAsync(int id)
    {
        var option = _machineryOptions.FirstOrDefault(o => o.Id == id);
        if (option != null)
        {
            _machineryOptions.Remove(option); // Verwijder uit de lijst
        }
        return Task.CompletedTask;
    }

    public Task<MachineryOptionDto.Detail> GetMachineryOptionAsync(int id)
    {

		var option = _machineryOptions.FirstOrDefault(o => o.Id == id);
        if (option is null)
        {
            throw new Exception("MachineryOption not found");
        }
        return Task.FromResult(option);
    }

    public Task<IEnumerable<MachineryOptionDto.Detail>> GetMachineryOptionsAsync()
    {
        return Task.FromResult(_machineryOptions.AsEnumerable());
    }

    public Task<List<MachineryOptionDto.Detail>> ImportPriceUpdateFile(string fileBase64)
    {
        var decodedFile = Convert.FromBase64String(fileBase64);

        var importedOptions = Enumerable.Range(1, 3).Select(i => new MachineryOptionDto.Detail
        {
            Id = _machineryOptions.Count + i,
            Price = 1500.00m + i, 
            Machinery = new MachineryDto.Index
            {
                Id = i,
                Name = $"Imported Machinery {i}",
                SerialNumber = $"ImportedSerialNumber {i}",
                Type = new MachineryTypeDto.Index { Id = i, Name = $"Imported Type {i}" },
                Description = $"Imported Description {i}"
            },
            Option = new OptionDto.Index
            {
                Id = i,
                Name = $"Imported Option {i}",
                Code = $"I{i}"
            }
        }).ToList();

        _machineryOptions.AddRange(importedOptions);

        return Task.FromResult(importedOptions);
    }

    public Task<MachineryOptionDto.Detail> UpdateMachineryOptionAsync(int id, MachineryOptionDto.Update machineryOptionDto)
    {
        var option = _machineryOptions.FirstOrDefault(o => o.Id == id);
        if (option != null)
        {
            option.Price = machineryOptionDto.Price;
            option.Machinery = new MachineryDto.Index
            {
                Id = machineryOptionDto.MachineryId,
                Name = $"Machinery {machineryOptionDto.MachineryId}",
                SerialNumber = $"SerialNumber {machineryOptionDto.MachineryId}",
                Type = new MachineryTypeDto.Index { Id = machineryOptionDto.MachineryId, Name = $"Type {machineryOptionDto.MachineryId}" },
                Description = $"Description {machineryOptionDto.MachineryId}"
            };
            option.Option = new OptionDto.Index
            {
                Id = machineryOptionDto.OptionId,
                Name = $"Option {machineryOptionDto.OptionId}",
                Code = $"O{machineryOptionDto.OptionId}"
            };
        }
        return Task.FromResult(option!);
    }
}
