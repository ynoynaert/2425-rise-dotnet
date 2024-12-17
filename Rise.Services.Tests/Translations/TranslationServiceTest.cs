using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rise.Persistence;
using Rise.Services.Translations;
using Rise.Shared.Helpers;
using Rise.Shared.Translations;

namespace Rise.Services.Tests.Translations;

public class TranslationServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly TranslationService _translationService;
    private readonly IConfiguration _configuration;

	public TranslationServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();

        _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "DeepL:APIKey", "fake-test-key" }
        })
        .Build();
			
        _translationService = new TranslationService(_context, _configuration!);  
    }

    [Fact]
    public async void GetTranslationsAsync_ShouldReturnAllTranslations()
    {
        // Arrange
        var translations = _context.Translations.Where(x => !x.IsDeleted).ToList();

        // Act
        var result = await _translationService.GetTranslationsAsync();

        // Assert
        Assert.Equal(translations.Count, result.Count());

    }

    [Fact]
    public async void UpdateTranslationAsync_ShouldUpdateTranslation()
    {
        // Arrange
        var translation = _context.Translations.FirstOrDefault();
        string userEmail = "test";

		var translationDto = new TranslationDto.Index
        {
            Id = translation!.Id,
            OriginalText = translation.OriginalText,
            TranslatedText = "Updated Translated Text",
            IsAccepted = true,
			UserEmail = "test"
		};

        // Act
        await _translationService.UpdateTranslationAsync(translationDto, userEmail);

        // Assert
        var updatedTranslation = _context.Translations.FirstOrDefault(t => t.Id == translationDto.Id);

        Assert.NotNull(updatedTranslation);
        Assert.Equal("Updated Translated Text", updatedTranslation.TranslatedText);
        Assert.True(updatedTranslation.IsAccepted);
    }

    [Fact]
    public async void DeleteTranslationAsync_ShouldDeleteTranslation()
    {
        // Arrange
        var translation = _context.Translations.FirstOrDefault();

        // Act
        await _translationService.DeleteTranslationAsync(translation!.Id);

        // Assert
        var deletedTranslation = _context.Translations.FirstOrDefault(t => t.Id == translation!.Id);

        Assert.Null(deletedTranslation);
    }


    [Fact]
    public async void GetAcceptedTranslationsAsync_ShouldReturnAcceptedTranslations()
    {
        // Arrange
        const int pageSize = 5;
        const int pageNumber = 1;

        var queryObject = new TranslationQueryObject
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        var acceptedTranslations = _context.Translations
            .Where(x => !x.IsDeleted && x.IsAccepted)
            .ToList();

        var expectedCount = acceptedTranslations.Skip((pageNumber - 1) * pageSize).Take(pageSize).Count();

        // Act
        var result = await _translationService.GetAcceptedTranslationsAsync(queryObject);

        // Assert
        Assert.Equal(expectedCount, result.Count());
    }

    [Fact]
    public async void GetUnacceptedTranslationsAsync_ShouldReturnUnacceptedTranslations()
    {
        // Arrange
        const int pageSize = 5;
        const int pageNumber = 1;

        var queryObject = new UnacceptedTranslationQueryObject
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        var acceptedTranslations = _context.Translations
            .Where(x => !x.IsDeleted && !x.IsAccepted)
            .ToList();

        var expectedCount = acceptedTranslations.Skip((pageNumber - 1) * pageSize).Take(pageSize).Count();

        // Act
        var result = await _translationService.GetUnacceptedTranslationsAsync(queryObject);

        // Assert
        Assert.Equal(expectedCount, result.Count());
    }

    [Fact]
    public async void GetUnacceptedTranslationsAsync_ShouldReturnUnacceptedTranslations_WithPaging()
    {
        // Arrange
        var queryObject = new UnacceptedTranslationQueryObject { PageNumber = 1, PageSize = 1 };
        var translations = _context.Translations.Where(x => !x.IsDeleted && !x.IsAccepted).Skip((queryObject.PageNumber - 1) * queryObject.PageSize).Take(queryObject.PageSize).ToList();

        // Act
        var result = await _translationService.GetUnacceptedTranslationsAsync(queryObject);

        // Assert
        Assert.Equal(translations.Count, result.Count());
    }

    [Fact]
    public async void GetTranslationAsync_ShouldReturnTranslation()
    {
        // Arrange
        var translation = _context.Translations.FirstOrDefault();

        // Act
        var result = await _translationService.GetTranslationAsync(translation!.Id);

        // Assert
        Assert.Equal(translation!.TranslatedText, result.TranslatedText);
    }

    [Fact]
    public async Task CreateTranslationAsync_ShouldCreateTranslation()
    {
        // Arrange
        string userEmail = "test";

		var translation = new TranslationDto.Index
        {
            OriginalText = "Test Original Text",
            TranslatedText = "Test Translated Text",
            IsAccepted = false,
            UserEmail = "test"
		};

        // Act
        var createdTranslation = await _translationService.CreateTranslationAsync(translation, userEmail);

        // Assert
        var savedTranslation = _context.Translations.SingleOrDefault(t => t.OriginalText == "Test Original Text");
        Assert.NotNull(savedTranslation);
        Assert.Equal("Test Original Text", savedTranslation?.OriginalText);
        Assert.Equal("Test Translated Text", savedTranslation?.TranslatedText);
        Assert.Equal(false, savedTranslation?.IsAccepted);
    }

    [Fact]
    public async void TranslateText_ShouldThrowException_WhenTextIsNullOrEmpty()
    {
        // Arrange
        var text = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _translationService.TranslateText(text));
    }

    [Fact]
    public async void TranslateText_ShouldThrowException_WhenTextIsWhiteSpace()
    {
        // Arrange
        var text = " ";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _translationService.TranslateText(text));
    }

    [Fact]
    public async void TranslateText_ShouldThrowException_WhenTextIsNull()
    {
        // Arrange
        string? text = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _translationService.TranslateText(text!));
    }

    [Fact]
    public async void GetTotalAcceptedTranslationsCountAsync_ShouldReturnTotalAcceptedTranslationsCount()
    {
        // Arrange
        var translations
            = _context.Translations
            .Where(x => !x.IsDeleted && x.IsAccepted)
            .Count();

        // Act
        var result = await _translationService.GetTotalAcceptedTranslationsAsync();
        Assert.Equal(translations, result);

    }

    [Fact]
    public async void GetTotalUnacceptedTranslationsCountAsync_ShouldReturnTotalUnacceptedTranslationsCount()
    {
        // Arrange
        var translations
            = _context.Translations
            .Where(x => !x.IsDeleted && !x.IsAccepted)
            .Count();

        // Act
        var result = await _translationService.GetTotalUnacceptedTranslationsAsync();
        Assert.Equal(translations, result);

    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
