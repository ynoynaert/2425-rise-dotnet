using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Domain.Machineries;

public class MachineryType : Entity
{
    private string name = default!;
    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    private readonly List<Machinery> machineries = [];
    public IReadOnlyList<Machinery> Machineries => machineries.AsReadOnly();

    public void AddMachinery(Machinery machinery)
    {
        if (machineries.Any(m => m.Id == machinery.Id))
        {
            return;
        }

        machineries.Add(machinery);
    }
}
