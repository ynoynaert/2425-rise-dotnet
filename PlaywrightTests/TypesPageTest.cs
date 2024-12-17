using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace Rise.PlaywrightTests
{
    public class TypesPageTest
    {
        private async Task Login(IPage page, string username, string password)
        {
            await page.GotoAsync("https://localhost:5001/machines");

            await page.FillAsync("input[id=\"username\"]", username);
            await page.FillAsync("input[id=\"password\"]", password);

            await page.ClickAsync("button:has-text('Continue'):not([aria-hidden='true'])");

            await page.GotoAsync("https://localhost:5001/machinetypes");
        }

        [Fact]
        public async Task AdminCanSeeTypesPage()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await Login(page, "pieter.desmet@dozer.be", "Wachtwoord123");

            var title = await page.Locator("h1").TextContentAsync();
            Assert.Equal("Types", title);

            await browser.CloseAsync();
        }

        [Fact]
        public async Task AdminCanSeeButtons()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await Login(page, "pieter.desmet@dozer.be", "Wachtwoord123");

            var voegToeButton = await page.TextContentAsync(".btn-primary");

            Assert.Contains("Voeg type toe", voegToeButton);
           
            await browser.CloseAsync();
        }

        [Fact]
        public async Task VerkoperCantSeePage()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await Task.Delay(500);

            await Login(page, "noah.goossens@dozer.be", "Wachtwoord123");
            await Task.Delay(500);

            var title = await page.TextContentAsync("h3.h3");

            Assert.Equal("Oeps! Sorry, we kunnen deze pagina niet meer vinden", title);

            await browser.CloseAsync();
        }
    }
}
