using System.Net.Http.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;

namespace Rise.Client.Machineries.Services;

public class MachineryOptionService : IMachineryOptionService
{

    private readonly HttpClient httpClient;

    public MachineryOptionService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<MachineryOptionDto.Detail> CreateMachineryOptionAsync(MachineryOptionDto.Create machineryOptionDto)
    {
        var response = await httpClient.PostAsJsonAsync("MachineryOption", machineryOptionDto);
        var machineryOption = await response.Content.ReadFromJsonAsync<MachineryOptionDto.Detail>();
        return machineryOption!;
    }

    public async Task DeleteMachineryOptionAsync(int id)
    {
        await httpClient.DeleteAsync($"MachineryOption/{id}");
    }

    public async Task<MachineryOptionDto.Detail> GetMachineryOptionAsync(int id)
    {
        var machineryOption = await httpClient.GetFromJsonAsync<MachineryOptionDto.Detail>($"MachineryOption/{id}");
        return machineryOption!;
    }

    public async Task<IEnumerable<MachineryOptionDto.Detail>> GetMachineryOptionsAsync()
    {
        var machineryOptions = await httpClient.GetFromJsonAsync<IEnumerable<MachineryOptionDto.Detail>>("MachineryOption");
        return machineryOptions!;
    }

    public async Task<List<MachineryOptionDto.Detail>> ImportPriceUpdateFile(string fileBase64)
    {
        var fileUploadModel = new { FileBase64 = fileBase64 };
        var response = await httpClient.PostAsJsonAsync("MachineryOption/import", fileUploadModel);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<MachineryOptionDto.Detail>>() ?? new List<MachineryOptionDto.Detail>();

    }

    public async Task<MachineryOptionDto.Detail> UpdateMachineryOptionAsync(int id, MachineryOptionDto.Update machineryOptionDto)
    {
        await httpClient.PutAsJsonAsync($"MachineryOption/", machineryOptionDto);
        var machineryOption = await httpClient.GetFromJsonAsync<MachineryOptionDto.Detail>($"MachineryOption/{id}");
        return machineryOption!;
    }
}
