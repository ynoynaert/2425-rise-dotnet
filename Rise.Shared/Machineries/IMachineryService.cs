using Rise.Shared.Helpers;

namespace Rise.Shared.Machineries;
public interface IMachineryService
{
    Task<IEnumerable<MachineryDto.Detail>> GetMachineriesAsync(MachineryQueryObject query);
    Task<MachineryDto.Detail> GetMachineryAsync(int id);
    Task<MachineryResult.Create> CreateMachineryAsync(MachineryDto.Create machineryDto);
    Task<MachineryResult.Create> UpdateMachineryAsync(int id, MachineryDto.Update machineryDto);
    Task DeleteMachineryAsync(int id);
    Task<MachineryDto.XtremeDetail> GetMachineryAsyncWithCategories(int id, OptionQueryObject query);
    Task<int> GetTotalMachineriesAsync();
    Task<MachineryDto.Index> GetMachineryByMachineryNameAsync(string machineryName);
    Task DeleteImageMachineryAsync(int id, int imageId);
}
