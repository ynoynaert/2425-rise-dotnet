using Microsoft.Playwright;

namespace Rise.PlaywrightTests;

public class QuotesPageTest
{
    private async Task LoginAsAdmin(IPage page)
    {
        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

    private async Task LoginAsVerkoper(IPage page)
    {
        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

    [Fact]
    public async Task ShowQuotesPage_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes");

        await LoginAsAdmin(page);

        await page.WaitForSelectorAsync("h1");

        Assert.True(await page.IsVisibleAsync("h1"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DoNotShowQuotesPage_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes");

        await LoginAsVerkoper(page);

        await page.WaitForSelectorAsync("h1");

        Assert.True(await page.IsVisibleAsync("h1"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToAddQuote_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes/toevoegen");

        await LoginAsAdmin(page);

        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToAddQuote_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes/toevoegen");

        await LoginAsVerkoper(page);

        await page.WaitForSelectorAsync("h3");

        Assert.True(await page.IsVisibleAsync("h3"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToDetail_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes/");

        await LoginAsAdmin(page);

        await page.WaitForSelectorAsync("h1");

        await page.ClickAsync("td:first-of-type");

        Assert.True(await page.IsVisibleAsync("h4"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToDetail_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertes");

        await LoginAsVerkoper(page);

        await page.WaitForSelectorAsync("h1");

        await page.ClickAsync("td:first-of-type");
        await page.WaitForSelectorAsync("h3");

        Assert.True(await page.IsVisibleAsync("h3"));

        await browser.CloseAsync();
    }
}
