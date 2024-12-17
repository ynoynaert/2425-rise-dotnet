using Microsoft.Playwright;


namespace Rise.PlaywrightTests;

public class OrdersPageTest
{

    [Fact]
    public async Task RenderOrderTable_AsAdministrator()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/bestellingen");

        await page.FillAsync("input[id=\"username\"]", "aria.maes@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");


        await page.WaitForSelectorAsync("h1");
        Assert.True(await page.IsVisibleAsync("h1:has-text(\"Bestellingen\")"));    

        await browser.CloseAsync();
    }

    [Fact]
    public async Task RenderOrderTable_AsSalesperson()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/bestellingen");

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");


        await page.WaitForSelectorAsync("h1");
        Assert.True(await page.IsVisibleAsync("h1:has-text(\"Bestellingen\")"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task RenderOrder_AsAdministrator()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/bestellingen");

        await page.FillAsync("input[id=\"username\"]", "aria.maes@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.WaitForSelectorAsync("h1");

        Assert.True(await page.IsVisibleAsync("h1:has-text(\"Bestellingen\")"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task RenderOrder_AsSalesperson()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/bestellingen");

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");


        await page.WaitForSelectorAsync("h1");

        Assert.True(await page.IsVisibleAsync("h1:has-text(\"Bestellingen\")"));

        await browser.CloseAsync();
    }

}
