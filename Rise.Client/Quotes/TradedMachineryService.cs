using System.Net.Http.Json;
using Rise.Shared.Quotes;

namespace Rise.Client.Quotes;

public class TradedMachineryService(HttpClient httpClient) : ITradedMachineryService
{
	public async Task<TradedMachineryResult.Create> CreateTradedMachineryAsync(TradedMachineryDto.Create tradedMachineryDto)
	{
		var response = await httpClient.PostAsJsonAsync("tradedMachinery", tradedMachineryDto);
		var tradedMachinery = await response.Content.ReadFromJsonAsync<TradedMachineryResult.Create>();
		return tradedMachinery!;
	}
}
