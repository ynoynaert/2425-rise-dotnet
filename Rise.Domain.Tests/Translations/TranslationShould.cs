using System;
using Rise.Domain.Translations;
using Shouldly;
using Xunit;

namespace Rise.Domain.Tests.Translations;

public class TranslationShould
{
    [Fact]
    public void BeCreatedWithValidData()
    {
        Translation t = new()
        {
            OriginalText = "Übersetzungstest",
            TranslatedText = "Vertalingstest",
            IsAccepted = false,
			UserEmail = "test"
		};

        t.OriginalText.ShouldBe("Übersetzungstest");
        t.TranslatedText.ShouldBe("Vertalingstest");
        t.IsAccepted.ShouldBeFalse();
    }

    [Fact]
    public void AllowAcceptingTranslation()
    {
        Translation t = new()
        {
            OriginalText = "Test",
            TranslatedText = "TestVertaling",
            IsAccepted = false,
			UserEmail = "test"
		};

        t.IsAccepted = true;

        t.IsAccepted.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidOriginalText(string? originalText)
    {
        Action act = () =>
        {
            Translation t = new()
            {
                OriginalText = originalText!,
                TranslatedText = "ValidTranslation",
                IsAccepted = false,
				UserEmail = "test"
			};
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidTranslatedText(string? translatedText)
    {
        Action act = () =>
        {
            Translation t = new()
            {
                OriginalText = "ValidOriginalText",
                TranslatedText = translatedText!,
                IsAccepted = false,
				UserEmail = "test"
			};
        };

        act.ShouldThrow<ArgumentException>();
    }

	[Theory]
	[InlineData(null)]
	[InlineData("   ")]
	[InlineData("")]
	public void NotBeCreatedWithAnEmptyUserId(string? userId)
	{
		Action act = () =>
		{
			Translation t = new()
			{
				OriginalText = "ValidOriginalText",
				TranslatedText = "translatedText",
				IsAccepted = false,
				UserEmail = userId!
			};
		};

		act.ShouldThrow<ArgumentException>();
	}

	[Fact]
    public void AllowUpdatingOriginalAndTranslatedText()
    {
        Translation t = new()
        {
            OriginalText = "OldOriginal",
            TranslatedText = "OldTranslation",
            IsAccepted = false,
			UserEmail = "test"
		};

        t.OriginalText = "NewOriginal";
        t.TranslatedText = "NewTranslation";
        
        t.OriginalText.ShouldBe("NewOriginal");
        t.TranslatedText.ShouldBe("NewTranslation");
    }
}
