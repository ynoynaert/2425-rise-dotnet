using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.FakeServices;

internal class FakeMachineryTypeService : IMachineryTypeService
{
    private List<MachineryTypeDto.Index> _allTypes;

    public FakeMachineryTypeService()
    {
        _allTypes = Enumerable.Range(1, 5)
                              .Select(i => new MachineryTypeDto.Index { Id = i, Name = $"Type {i}" })
                              .ToList();
    }

    public Task<MachineryTypeDto.Index> CreateMachineryTypeAsync(MachineryTypeDto.Create typeDto)
    {
        var newId = _allTypes.Count + 1;
        var newType = new MachineryTypeDto.Index
        {
            Id = newId,
            Name = typeDto.Name!
        };
        _allTypes.Add(newType);
        return Task.FromResult(newType);
    }

    public Task DeleteMachineryTypeAsync(int id)
    {
        var typeToRemove = _allTypes.FirstOrDefault(t => t.Id == id);
        if (typeToRemove != null)
        {
            _allTypes.Remove(typeToRemove);
        }
        return Task.CompletedTask;
    }

    public Task<MachineryTypeDto.Index> GetMachineryTypeAsync(int id)
    {
        var type = _allTypes.FirstOrDefault(t => t.Id == id);
        if (type is null)
        {
            throw new Exception("Type not found");
        }
        return Task.FromResult(type);
    }

    public Task<IEnumerable<MachineryTypeDto.Index>> GetMachineryTypesAsync()
    {
        return Task.FromResult(_allTypes.AsEnumerable());
    }

    public Task<MachineryTypeDto.Index> UpdateMachineryTypeAsync(int id, MachineryTypeDto.Update typeDto)
    {
        var existingType = _allTypes.FirstOrDefault(t => t.Id == id);
        if (existingType != null)
        {
            existingType.Name = typeDto.Name!;
        }
        return Task.FromResult(existingType!);
    }
}
