using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Translations;
using Rise.Persistence;
using Rise.Shared.Machineries;
using Rise.Shared.Translations;
using System.Text.Json;
using System.Net.Http.Headers;
using Rise.Shared.Helpers;
using Auth0.ManagementApi;
using Serilog;

namespace Rise.Services.Translations;

public class TranslationService(ApplicationDbContext dbContext, IConfiguration configuration) : ITranslationService
{
	private readonly ApplicationDbContext dbContext = dbContext;
	private readonly string deepLApiKey = configuration["DeepL:APIKey"] ?? throw new InvalidOperationException("DeepL API key is not configured.");

	public async Task<IEnumerable<TranslationDto.Index>> GetTranslationsAsync()
	{
		IQueryable<TranslationDto.Index> query = dbContext.Translations
			.Where(x => !x.IsDeleted)
			.Select(x => new TranslationDto.Index
			{
				Id = x.Id,
				OriginalText = x.OriginalText,
				TranslatedText = x.TranslatedText,
				IsAccepted = x.IsAccepted
			});

		var translations = await query.ToListAsync();
		Log.Information("Translations retrieved");
        return translations ?? [];
	}

	public async Task<IEnumerable<TranslationDto.Index>> GetAcceptedTranslationsAsync(TranslationQueryObject queryObject)
	{
		IQueryable<TranslationDto.Index> query = dbContext.Translations
			.Where(x => !x.IsDeleted && x.IsAccepted)
			.Select(x => new TranslationDto.Index
			{
				Id = x.Id,
				OriginalText = x.OriginalText,
				TranslatedText = x.TranslatedText,
				IsAccepted = x.IsAccepted,
				UserEmail = x.UserEmail
			});

		if (!string.IsNullOrWhiteSpace(queryObject.Search))
		{
			string search = queryObject.Search.ToLower();
			query = query.Where(x => x.OriginalText.ToLower().Contains(search) ||
									 x.TranslatedText.ToLower().Contains(search) ||
									 x.UserEmail!.ToLower().Contains(search));
		}

		int totalItems = await query.CountAsync();

		int skip = (queryObject.PageNumber - 1) * queryObject.PageSize;
		query = query.Skip(skip).Take(queryObject.PageSize);

		var translations = await query.ToListAsync();

		queryObject.HasNext = (skip + translations.Count) < totalItems;

		Log.Information("Accepted translations retrieved");
        return translations;
	}


	public async Task<IEnumerable<TranslationDto.Index>> GetUnacceptedTranslationsAsync(UnacceptedTranslationQueryObject queryObject)
	{
		IQueryable<TranslationDto.Index> query = dbContext.Translations
			.Where(x => !x.IsDeleted && !x.IsAccepted)
			.Select(x => new TranslationDto.Index
			{
				Id = x.Id,
				OriginalText = x.OriginalText,
				TranslatedText = x.TranslatedText,
				IsAccepted = x.IsAccepted
			});

		int totalItems = await query.CountAsync();

		int skip = (queryObject.PageNumber - 1) * queryObject.PageSize;
		query = query.Skip(skip).Take(queryObject.PageSize);

		var translations = await query.ToListAsync();

		queryObject.HasNext = (skip + translations.Count) < totalItems;
		Log.Information("Unaccepted translations retrieved");

        return translations;
	}

	public async Task<TranslationDto.Index> CreateTranslationAsync(TranslationDto.Index translationDto, string userEmail)
	{
		var translation = new Translation
		{
			OriginalText = translationDto.OriginalText,
			TranslatedText = translationDto.TranslatedText,
			IsAccepted = translationDto.IsAccepted,
			UserEmail = userEmail
		};

		dbContext.Translations.Add(translation);
		await dbContext.SaveChangesAsync();
		Log.Information("Translation created");

        return translationDto;
	}

	public async Task<TranslationDto.Index> GetTranslationAsync(int id)
	{
		var translation = await dbContext.Translations
			.Where(x => !x.IsDeleted)
			.Select(x => new TranslationDto.Index
			{
				Id = x.Id,
				OriginalText = x.OriginalText,
				TranslatedText = x.TranslatedText,
				IsAccepted = x.IsAccepted,
				UserEmail = x.UserEmail
			})
			.SingleAsync(x => x.Id == id);
		Log.Information("Translation retrieved by id");

        return translation;
	}

	public async Task<TranslationDto.Index> UpdateTranslationAsync(TranslationDto.Index translationDto, string userEmail)
	{
		var translation = await dbContext.Translations.SingleOrDefaultAsync(x => x.Id == translationDto.Id) ?? throw new InvalidOperationException($"Translation with Id {translationDto.Id} was not found.");
		translation.OriginalText = translationDto.OriginalText;
		translation.TranslatedText = translationDto.TranslatedText;
		translation.IsAccepted = translationDto.IsAccepted;
		translation.UserEmail = userEmail;	

		await dbContext.SaveChangesAsync();
        Log.Information("Translation updated");
        return translationDto;
	}

	public async Task DeleteTranslationAsync(int id)
	{
		var translation = await dbContext.Translations.SingleAsync(x => x.Id == id);

		dbContext.Translations.Remove(translation);
		Log.Information("Translation deleted");
        await dbContext.SaveChangesAsync();
	}

	public async Task<string> TranslateText(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			Log.Warning("Text to translate is required.");
            throw new ArgumentException("Text to translate is required.", nameof(text));
		}

		var existingTranslation = await dbContext.Translations
			.Where(t => t.OriginalText == text && !t.IsDeleted)
			.Select(t => t.TranslatedText)
			.FirstOrDefaultAsync();

		if (existingTranslation != null)
		{
			Log.Warning("Translation already exists");
            return existingTranslation;
		}

		if (string.IsNullOrEmpty(deepLApiKey))
		{
            Log.Warning("DeepL API key is not configured.");
            throw new InvalidOperationException("DeepL API key is not configured.");
		}

		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

		var requestContent = new Dictionary<string, string>
	{
		{ "auth_key", deepLApiKey },
		{ "text", text.Trim() },
		{ "target_lang", "NL" }
	};

		var response = await httpClient.PostAsync("https://api-free.deepl.com/v2/translate", new FormUrlEncodedContent(requestContent));
		response.EnsureSuccessStatusCode();

		var jsonResponse = await response.Content.ReadAsStringAsync();
		var translationResponse = JsonSerializer.Deserialize<DeepResponse>(jsonResponse);

		var detectedSourceLanguage = translationResponse?.translations?.FirstOrDefault()?.detected_source_language;
		var translatedText = translationResponse?.translations?.FirstOrDefault()?.text ?? "Translation not available";

		if (detectedSourceLanguage == "DE" && !string.IsNullOrEmpty(translatedText))
		{
			Log.Information("Translation retrieved from DeepL");
            var newTranslation = new Translation
			{
				OriginalText = text,
				TranslatedText = translatedText,
				IsAccepted = false,
			};

			dbContext.Translations.Add(newTranslation);
			await dbContext.SaveChangesAsync();
			Log.Information("Translation saved to database");
        }

		Log.Information("Translation retrieved from DeepL");
        return translatedText ?? text;
	}

	public class DeepResponse
	{
		public List<DeepTranslation> translations { get; set; } = new List<DeepTranslation>();
	}

	public class DeepTranslation
	{
		public string detected_source_language { get; set; } = String.Empty;
		public string text { get; set; } = String.Empty;
	}

	public async Task<int> GetTotalAcceptedTranslationsAsync()
	{
		int totalAccepted = await dbContext.Translations
			.Where(x => !x.IsDeleted && x.IsAccepted)
			.CountAsync();

		Log.Information("Total accepted translations retrieved");
        return totalAccepted;
	}

	public async Task<int> GetTotalUnacceptedTranslationsAsync()
	{
		int totalUnaccepted = await dbContext.Translations
			.Where(x => !x.IsDeleted && !x.IsAccepted)
			.CountAsync();

        Log.Information("Total unaccepted translations retrieved");
        return totalUnaccepted;
	}
}