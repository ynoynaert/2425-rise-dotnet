using Xunit;
using Xunit.Abstractions;
using System;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Rise.PlaywrightTests;

public class CategoryTableTest
{
    [Fact]
    public async Task DisplayCategoriesForAdministrator()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");
        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.WaitForSelectorAsync(".table-group-cell");

        var categoryRows = await page.Locator(".table-group-cell").CountAsync();
        Assert.True(categoryRows > 0);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DisplayCategoriesForVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");
        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.WaitForSelectorAsync(".table-group-cell");

        var categoryRows = await page.Locator(".table-group-cell").CountAsync();
        Assert.True(categoryRows > 0);

        await browser.CloseAsync();
    }
    
    [Fact]
    public async Task NavigateToAddCategoryPageAsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");
        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.ClickAsync("button:has-text('Voeg categorie toe')");

        var currentUrl = page.Url;
        Assert.Contains("/categorie%C3%ABn/toevoegen", currentUrl);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToAddCategoryPageAsVerkoperFails()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn/toevoegen");
        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task NavigateToDetailCategoryPage()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");
        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        await page.DblClickAsync("td:has-text(\"1200 - Belettering / Blinderen\")");

        var currentUrl = page.Url;
        Assert.Contains("/categorie%C3%ABn/1", currentUrl);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DetailCategoryPageShowsButtonsForAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn/1");
        await page.FillAsync("input[id=\"username\"]", "pieter.desmet@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        var addOptionBtn = await page.TextContentAsync(".ml-auto");

        Assert.Contains("Voeg optie toe", addOptionBtn);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DetailCategoryPageShowsNoButtonsForVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn/1");
        await page.FillAsync("input[id=\"username\"]", "noah.goossens@dozer.be");
        await page.FillAsync("input[id=\"password\"]", "Wachtwoord123");
        await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

        var buttonLocator = page.Locator(".ml-auto");
        var isButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isButtonVisible);

        var editOptionBtnLocator = page.Locator("button.btn-secondary:has-text('Bewerken')");
        var isEditOptionBtnVisible = await editOptionBtnLocator.IsVisibleAsync();
        Assert.False(isEditOptionBtnVisible);

        var deleteOptionBtn = page.Locator("button.btn-secondary:has-text('Verwijderen')");
        var isDeleteOptionBtn = await editOptionBtnLocator.IsVisibleAsync();
        Assert.False(isEditOptionBtnVisible);

        var editCategoryBtn = page.Locator("button.btn-secondary:has-text('Categorie bewerken')");
        var isEditCategoryBtn = await editOptionBtnLocator.IsVisibleAsync();
        Assert.False(isEditOptionBtnVisible);

        var deleteCategoryBtn = page.Locator(".btn-secondary:has-text('Categorie verwijderen')");
        var isDeleteCategoryBtn = await editOptionBtnLocator.IsVisibleAsync();
        Assert.False(isEditOptionBtnVisible);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task Verkoper_Cant_See_Button()
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

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");

        var buttonLocator = page.Locator(".btn-primary");
        var isButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isButtonVisible);

        await browser.CloseAsync();
    }

    [Fact]
    public async Task PriceUpdateButtonInvisibleForSalesPeople()
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

        await page.GotoAsync("https://localhost:5001/categorie%C3%ABn");

        var buttonLocator = page.Locator(".btn-priceupdate");
        var isButtonVisible = await buttonLocator.IsVisibleAsync();
        Assert.False(isButtonVisible);

        await browser.CloseAsync();
    }
}
