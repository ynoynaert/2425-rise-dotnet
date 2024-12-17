using Rise.Shared.Helpers;

namespace Rise.Shared.Translations;

public interface ITranslationService
{
    Task<IEnumerable<TranslationDto.Index>> GetTranslationsAsync();
    Task<IEnumerable<TranslationDto.Index>> GetAcceptedTranslationsAsync(TranslationQueryObject query);
    Task<IEnumerable<TranslationDto.Index>> GetUnacceptedTranslationsAsync(UnacceptedTranslationQueryObject query);
    Task<TranslationDto.Index> CreateTranslationAsync(TranslationDto.Index translationDto, string userEmail);
    Task<TranslationDto.Index> GetTranslationAsync(int id);
    Task<TranslationDto.Index> UpdateTranslationAsync(TranslationDto.Index translationDto, string userEmail);
    Task DeleteTranslationAsync(int id);
    Task<string> TranslateText(string text);
    Task<int> GetTotalAcceptedTranslationsAsync();
    Task<int> GetTotalUnacceptedTranslationsAsync();
}
