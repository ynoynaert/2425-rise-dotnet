using Rise.Shared.Translations;
using Rise.Shared.Helpers;
using System.Net.Http.Json;

namespace Rise.Client.Translations;

public class TranslationService(HttpClient httpClient) : ITranslationService
{
    private readonly HttpClient httpClient = httpClient;

	public Task<TranslationDto.Index> CreateTranslationAsync(TranslationDto.Index translationDto, string userId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTranslationAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TranslationDto.Index>> GetAcceptedTranslationsAsync(TranslationQueryObject query)
    {
        string url = $"translation/accepted?Search={query.Search}&PageNumber={query.PageNumber}";
        var translations = await httpClient.GetFromJsonAsync<IEnumerable<TranslationDto.Index>>(url);
        return translations!;
    }

    public Task<IEnumerable<TranslationDto.Index>> GetUnacceptedTranslationsAsync(UnacceptedTranslationQueryObject query)
    {
        string url = $"translation/unaccepted?PageNumber={query.PageNumber}";
        var translations = httpClient.GetFromJsonAsync<IEnumerable<TranslationDto.Index>>(url);
        return translations!;
    }

    public Task<int> GetTotalAcceptedTranslationsAsync()
    {
        return httpClient.GetFromJsonAsync<int>("translation/totalAccepted");
    }

    public Task<int> GetTotalUnacceptedTranslationsAsync()
    {
        return httpClient.GetFromJsonAsync<int>("translation/totalUnaccepted");
    }

    public Task<TranslationDto.Index> GetTranslationAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TranslationDto.Index>> GetTranslationsAsync()
    {
        var translations = await httpClient.GetFromJsonAsync<IEnumerable<TranslationDto.Index>>("translation");
        return translations!;
    }



    public async Task<string> TranslateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text to translate is required.", nameof(text));
        }

        var response = await httpClient.PostAsJsonAsync("translation/translate", text);
        response.EnsureSuccessStatusCode();

        var translatedText = await response.Content.ReadAsStringAsync();
        return translatedText;
    }

    public async Task<TranslationDto.Index> UpdateTranslationAsync(TranslationDto.Index translationDto, string userId)
    {
        await httpClient.PutAsJsonAsync("translation", translationDto);
        return translationDto;
    }
}
