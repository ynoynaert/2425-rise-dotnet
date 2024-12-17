using Rise.Shared.Quotes;

namespace Rise.Shared.Machineries
{
    public interface IMachineryOptionService
    {
        Task<IEnumerable<MachineryOptionDto.Detail>> GetMachineryOptionsAsync();
        Task<MachineryOptionDto.Detail> GetMachineryOptionAsync(int id);
        Task<MachineryOptionDto.Detail> CreateMachineryOptionAsync(MachineryOptionDto.Create machineryOptionDto);
        Task<MachineryOptionDto.Detail> UpdateMachineryOptionAsync(int id, MachineryOptionDto.Update machineryOptionDto);
        Task DeleteMachineryOptionAsync(int id);
        Task<List<MachineryOptionDto.Detail>> ImportPriceUpdateFile(string fileBase64);
    }
}
