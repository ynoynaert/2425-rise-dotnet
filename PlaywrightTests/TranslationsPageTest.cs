using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rise.PlaywrightTests;

public class TranslationsPageTest
{
    private async Task LoginAndNavigateToTranslationsAsync_AsAdmin(IPage page, string url)
    {
        await page.GotoAsync(url);

        await page.FillAsync("input[id=\"username\"]", "aria.maes@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

    private async Task LoginAndNavigateToTranslationsAsync_AsVerkpoer(IPage page, string url)
    {
        await page.GotoAsync(url);

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

	[Fact]
    public async Task ShowUnacceptedTranslations_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAndNavigateToTranslationsAsync_AsAdmin(page, "https://localhost:5001/vertalingen");

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task ShowAcceptedTranslations_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAndNavigateToTranslationsAsync_AsAdmin(page, "https://localhost:5001/vertalingen/overzicht");

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DoNotShowUnacceptedTranslations_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAndNavigateToTranslationsAsync_AsVerkpoer(page, "https://localhost:5001/vertalingen");
        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DoNotShowAcceptedTranslations_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAndNavigateToTranslationsAsync_AsVerkpoer(page, "https://localhost:5001/vertalingen/overzicht");

        await page.WaitForSelectorAsync("h3");
        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }

}
