using Microsoft.Playwright;

namespace Rise.PlaywrightTests;

public class SalesPeoplePageTest
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
    public async Task ShowSalespeopleTable_AsAdmin()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/verkopers");

        await LoginAsAdmin(page);

        await page.WaitForSelectorAsync("th");

        Assert.True(await page.IsVisibleAsync("th"));

        await browser.CloseAsync();
    }

    [Fact]
    public async Task DoNotShowSalespeopleTable_AsVerkoper()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5001/verkopers");

        await LoginAsVerkoper(page);

        var title = await page.TextContentAsync("h3.h3");

        Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

        await browser.CloseAsync();
    }

	[Fact]
	public async Task SearchSalespeople_AsAdmin()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers");

		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("th");

		await page.FillAsync("input.filters.textfield", "Pieter");

		await page.WaitForSelectorAsync("td");

		Assert.True(await page.IsVisibleAsync("td"));

		await browser.CloseAsync();
	}

	[Fact]
	public async Task NavigateToAddSalesPerson_AsAdmin()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers");

		await LoginAsAdmin(page);

		await page.ClickAsync("text=Verkoper aanmelden");

		await page.WaitForSelectorAsync("h3");

		var inputElements = await page.QuerySelectorAllAsync("input");
		Assert.Equal(5, inputElements.Count);

		await browser.CloseAsync();
	}

	[Fact]
	public async Task AddSalesPersonForm_AllFieldsEmpty_ShowsValidationErrors()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers/toevoegen");
		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("h3");

		await page.ClickAsync("button[type=\"submit\"]");

		Assert.True(await page.IsVisibleAsync("text=Naam moet ingevuld zijn"));
		Assert.True(await page.IsVisibleAsync("text=Email moet ingevuld zijn"));
		Assert.True(await page.IsVisibleAsync("text=Telefoonnummer moet ingevuld zijn"));
		Assert.True(await page.IsVisibleAsync("text=Wachtwoord moet ingevuld zijn"));
		Assert.True(await page.IsVisibleAsync("text=Wachtwoord moet ingevuld zijn"));

		await browser.CloseAsync();
	}

	[Fact]
	public async Task AddSalesPersonForm_InvalidEmail_ShowsValidationError()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers/toevoegen");
		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("h3");

		await page.FillAsync("input[data-testid=\"form-email\"]", "ongeldig-email");
		await page.ClickAsync("button.btn-primary");

		Assert.True(await page.IsVisibleAsync("text=Gelieve een geldig emailadres in te geven"));

		await browser.CloseAsync();
	}

	[Fact]
	public async Task AddSalesPersonForm_InvalidPhoneNumber_ShowsValidationError()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers/toevoegen");
		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("h3");

		await page.FillAsync("input[data-testid=\"form-phonenr\"]", "12345");
		await page.ClickAsync("button.btn-primary");

		Assert.True(await page.IsVisibleAsync("text=Telefoonnummer moet beginnen met +31 en gevolgd worden door 9 cijfers"));

		await browser.CloseAsync();
	}

	[Fact]
	public async Task AddSalesPersonForm_InvalidPassword_ShowsValidationErrors()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers/toevoegen");
		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("h3");

		await page.FillAsync("input[data-testid=\"form-pswd\"]", "kort");
		await page.ClickAsync("button.btn-primary");
		Assert.True(await page.IsVisibleAsync("text=Wachtwoord moet minstens 8 karakters bevatten"));

		await page.FillAsync("input[data-testid=\"form-pswd\"]", "zonderhoofdletter");
		await page.ClickAsync("button.btn-primary");
		Assert.True(await page.IsVisibleAsync("text=Wachtwoord moet minstens 1 hoofdletter bevatten"));
		Assert.True(await page.IsVisibleAsync("text=Wachtwoord moet minstens 1 cijfer bevatten"));

		await browser.CloseAsync();
	}

	[Fact]
	public async Task AddSalesPersonForm_PasswordsDoNotMatch_ShowsValidationError()
	{
		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		var context = await browser.NewContextAsync();
		var page = await context.NewPageAsync();

		await page.GotoAsync("https://localhost:5001/verkopers/toevoegen");
		await LoginAsAdmin(page);

		await page.WaitForSelectorAsync("h3");

		await page.FillAsync("input[data-testid=\"form-pswd\"]", "Password1");
		await page.FillAsync("input[data-testid=\"form-pswdconfirm\"]", "Mismatch1");
		await page.ClickAsync("button.btn-primary");

		Assert.True(await page.IsVisibleAsync("text=Wachtwoorden komen niet overeen"));

		await browser.CloseAsync();
	}
}
