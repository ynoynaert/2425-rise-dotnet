using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.FakeServices;

public class FakeOptionService : IOptionService
{
    private List<OptionDto.Detail> _allOptions;

    public FakeOptionService()
    {
        // Initialize with some sample options
        _allOptions = Enumerable.Range(1, 5).Select(i => new OptionDto.Detail
        {
            Id = i,
            Name = $"Option {i}",
            Code = $"Code {i}",
            Category = new CategoryDto.Index { Id = i, Name = $"Category {i}", Code = $"Code {i}" }
        }).ToList();
    }

    public Task<IEnumerable<OptionDto.Detail>> GetOptionsAsync()
    {
        return Task.FromResult(_allOptions.AsEnumerable());
    }

    public Task<OptionDto.Detail> GetOptionAsync(int id)
    {
        var option = _allOptions.FirstOrDefault(o => o.Id == id);
        if (option is null)
        {
            throw new Exception("Option not found");
        }
        return Task.FromResult(option);
    }

    public Task<OptionDto.Detail> CreateOptionAsync(OptionDto.Create optionDto)
    {
        var newId = _allOptions.Count + 1;
        var newOption = new OptionDto.Detail
        {
            Id = newId,
            Name = optionDto.Name!,
            Code = $"Code {newId}",
            Category = new CategoryDto.Index
            {
                Id = optionDto.CategoryId,
                Name = $"Category {optionDto.CategoryId}",
                Code = $"Code {optionDto.CategoryId}"
            }
        };

        _allOptions.Add(newOption);
        return Task.FromResult(newOption);
    }

    public Task<OptionDto.Detail> UpdateOptionAsync(int id, OptionDto.Update optionDto)
    {
        var existingOption = _allOptions.FirstOrDefault(o => o.Id == id);
        if (existingOption != null)
        {
            existingOption.Name = optionDto.Name!;
            existingOption.Category = new CategoryDto.Index
            {
                Id = optionDto.CategoryId,
                Name = $"Category {optionDto.CategoryId}",
                Code = $"Code {optionDto.CategoryId}"
            };
        }

        return Task.FromResult(existingOption!);
    }

    public Task DeleteOptionAsync(int id)
    {
        var optionToRemove = _allOptions.FirstOrDefault(o => o.Id == id);
        if (optionToRemove != null)
        {
            _allOptions.Remove(optionToRemove);
        }
        return Task.CompletedTask;
    }
}
