using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Machineries;
public interface IMachineryTypeService
{    Task<IEnumerable<MachineryTypeDto.Index>> GetMachineryTypesAsync();
    Task<MachineryTypeDto.Index> GetMachineryTypeAsync(int id);
    Task<MachineryTypeDto.Index> CreateMachineryTypeAsync(MachineryTypeDto.Create typeDto);
    Task<MachineryTypeDto.Index> UpdateMachineryTypeAsync(int id, MachineryTypeDto.Update typeDto);
    Task DeleteMachineryTypeAsync(int id);
}
