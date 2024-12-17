using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.PlaywrightTests;

public class LocationPageTest
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
    public async Task ShowLocationsPage_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/vestigingen");

        await LoginAsAdmin(page);

        await page.WaitForSelectorAsync("h1");

        Assert.True(await page.IsVisibleAsync("h1"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DoNotShowLocationsPage_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/vestigingen");

        await LoginAsVerkoper(page);

        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }
}
