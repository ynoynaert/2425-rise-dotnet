namespace Rise.Shared.Machineries;
public interface IOptionService
{
    Task<IEnumerable<OptionDto.Detail>> GetOptionsAsync();
    Task<OptionDto.Detail> GetOptionAsync(int id);
    Task<OptionDto.Detail> CreateOptionAsync(OptionDto.Create optionDto);
    Task<OptionDto.Detail> UpdateOptionAsync(int id, OptionDto.Update optionDto);
    Task DeleteOptionAsync(int id);
}
