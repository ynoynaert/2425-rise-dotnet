using Rise.Shared.Helpers;
using System.Net.Http.Json;
using Azure;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.Services;

public class OptionService(HttpClient httpClient) : IOptionService
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<OptionDto.Detail> CreateOptionAsync(OptionDto.Create optionDto)
    {
        var response = await httpClient.PostAsJsonAsync("option", optionDto);
        var option = await response.Content.ReadFromJsonAsync<OptionDto.Detail>();
        return option!;
    }

    public async Task DeleteOptionAsync(int id)
    {
        await httpClient.DeleteAsync($"option/{id}");
    }

    public async Task<IEnumerable<OptionDto.Detail>> GetOptionsAsync()
    {
        var options = await httpClient.GetFromJsonAsync<IEnumerable<OptionDto.Detail>>("option");
        return options!;
    }

    public async Task<OptionDto.Detail> GetOptionAsync(int id)
    {
        var option = await httpClient.GetFromJsonAsync<OptionDto.Detail>($"option/{id}");
        return option!;
    }

    public async Task<OptionDto.Detail> UpdateOptionAsync(int id, OptionDto.Update optionDto)
    {
        var response = await httpClient.PutAsJsonAsync($"option/", optionDto);
        var option = await response.Content.ReadFromJsonAsync<OptionDto.Detail>();
        return option!;
    }

}
