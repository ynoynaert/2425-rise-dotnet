using Microsoft.Playwright;

namespace Rise.PlaywrightTests;

public class InquiriesPageTest
{
    private async Task LoginAsAdmin(IPage page)
    {
        await page.FillAsync("input[id=\"username\"]", "aria.maes@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

    private async Task LoginAsVerkpoer(IPage page)
    {
        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");
    }

    [Fact]
    public async Task ShowInquiriesAsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertevoorstellen");
        await LoginAsAdmin(page);

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task ShowInquiriesAsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertevoorstellen");
        await LoginAsVerkpoer(page);

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToDetailPageAsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertevoorstellen");
        await LoginAsAdmin(page);

        await page.ClickAsync("td:first-of-type");

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToDetailPageAsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/offertevoorstellen");
        await LoginAsVerkpoer(page);

        await page.ClickAsync("td:first-of-type");

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }
}
