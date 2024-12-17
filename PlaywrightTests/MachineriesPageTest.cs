using Microsoft.Playwright;

namespace Rise.PlaywrightTests;

public class MachineriesPageTest
{
    [Fact]
    public async Task RenderMachineriesPage_AsAdministrator()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/machines");

        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.WaitForSelectorAsync("tbody tr");

        var machineryRows = await page.Locator("tbody tr").CountAsync();
        Assert.Equal(10, machineryRows);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task RenderMachineriesPage_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/machines");

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.WaitForSelectorAsync("tbody tr");

        var machineryRows = await page.Locator("tbody tr").CountAsync();
        Assert.Equal(10, machineryRows);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task RenderMachineryDetail_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/machines/11");

        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.ClickAsync("h3:has-text(\"Caterpillar D8T\")");

        Assert.True(await page.IsVisibleAsync("h4:has-text(\"CATD8T-202310011\")"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task Verkoper_Cant_See_AddButton()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/machines");

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        var buttonLocator = page.Locator(".btn-primary");
        var isButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isButtonVisible);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task Verkoper_Cant_See_MutateButtons_DetailPage()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/machines/11");

        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        var buttonLocator = page.Locator(".btn-secondary");
        var isButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isButtonVisible);

        var deleteButtonLocator = page.Locator(".btn-outline-danger");
        var isDeleteButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isDeleteButtonVisible);

        var editButtonLocator = page.Locator(".opties-bewerken-knop");
        var isEditButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isEditButtonVisible);

        await browser.CloseAsync();
    }
}
