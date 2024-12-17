using Rise.Shared.Machineries;
using System.Net.Http.Json;

namespace Rise.Client.Machineries.Services;
public class MachineryTypeService(HttpClient httpClient) : IMachineryTypeService
{
    public async Task<MachineryTypeDto.Index> CreateMachineryTypeAsync(MachineryTypeDto.Create typeDto)
    {
        var response = await httpClient.PostAsJsonAsync("machinerytype", typeDto);
        var machineryType = await response.Content.ReadFromJsonAsync<MachineryTypeDto.Index>();
        return machineryType!;
    }

    public async Task DeleteMachineryTypeAsync(int id)
    {
        await httpClient.DeleteAsync($"machinerytype/{id}");
    }

    public async Task<MachineryTypeDto.Index> GetMachineryTypeAsync(int id)
    {
        var machineryType = await httpClient.GetFromJsonAsync<MachineryTypeDto.Index>($"machinerytype/{id}");
        return machineryType!;
    }

    public async Task<IEnumerable<MachineryTypeDto.Index>> GetMachineryTypesAsync()
    {
        var machinery = await httpClient.GetFromJsonAsync<IEnumerable<MachineryTypeDto.Index>>("machinerytype");
        return machinery!;
    }

    public async Task<MachineryTypeDto.Index> UpdateMachineryTypeAsync(int id, MachineryTypeDto.Update typeDto)
    {
        var response = await httpClient.PutAsJsonAsync($"machinerytype/", typeDto);
        var machineryType = await response.Content.ReadFromJsonAsync<MachineryTypeDto.Index>();
        return machineryType!;
    }
}