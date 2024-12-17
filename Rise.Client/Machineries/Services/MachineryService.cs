using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using System.Net.Http.Json;

namespace Rise.Client.Machineries.Services;
public class MachineryService(HttpClient httpClient) : IMachineryService
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<MachineryResult.Create> CreateMachineryAsync(MachineryDto.Create machineryDto)
    {

        var response = await httpClient.PostAsJsonAsync("machinery", machineryDto);
        var result = await response.Content.ReadFromJsonAsync<MachineryResult.Create>();
        return result!;
    }

    public async Task DeleteMachineryAsync(int id)
    {
        await httpClient.DeleteAsync($"machinery/{id}");
    }

    public async Task<IEnumerable<MachineryDto.Detail>> GetMachineriesAsync(MachineryQueryObject query)
    {
        string url = $"machinery?Search={query.Search}&PageNumber={query.PageNumber}&TypeIds={query.TypeIds}&SortBy={query.SortBy}&IsDescending={query.IsDescending}";
        var result = await httpClient.GetFromJsonAsync<IEnumerable<MachineryDto.Detail>>(url);
        return result ?? Enumerable.Empty<MachineryDto.Detail>();
    }

    public async Task<MachineryDto.Detail> GetMachineryAsync(int id)
    {
        var Machine = await httpClient.GetFromJsonAsync<MachineryDto.Detail>($"machinery/{id}");
        return Machine!;
    }

    public Task<int> GetTotalMachineriesAsync()
    {
        return httpClient.GetFromJsonAsync<int>("machinery/total");
    }

    public async Task<MachineryDto.XtremeDetail> GetMachineryAsyncWithCategories(int id, OptionQueryObject query)
    {
        string url = $"machinery/detail/{id}?Search={query.Search}";
        var Machine = await httpClient.GetFromJsonAsync<MachineryDto.XtremeDetail>(url);
        return Machine!;
    }

    public async Task<MachineryResult.Create> UpdateMachineryAsync(int id, MachineryDto.Update machineryDto)
    {
        var response = await httpClient.PutAsJsonAsync($"machinery/", machineryDto);
        var result = await response.Content.ReadFromJsonAsync<MachineryResult.Create>();
        return result!;
    }

    public async Task DeleteImageMachineryAsync(int id, int imageId)
    {
        await httpClient.DeleteAsync($"machinery/{id}/{imageId}");
    }

    public Task<MachineryDto.Index> GetMachineryByMachineryNameAsync(string machineryName)
    {
        throw new NotImplementedException();
    }
}
