using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.MachineryOption;
using Rise.Shared.Machineries;
using Shouldly;
using System.Collections.Generic;
using System;
using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;

namespace Rise.Client.Machineries.MachineryOptionTests;

public class UpdatePriceShould : TestContext
{
    public UpdatePriceShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons();

        Services.AddScoped<IMachineryOptionService, FakeMachineryOptionService>();
    }

    [Fact]
    public async Task ShouldImportPriceUpdateFileCorrectly()
    {
        // Arrange
        var service = Services.GetRequiredService<IMachineryOptionService>();

        // Example base64 file string (not actual content, for test purposes)
        var fileBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("dummy-content"));

        // Act
        var result = await service.ImportPriceUpdateFile(fileBase64);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<List<MachineryOptionDto.Detail>>();
        result.Count.ShouldBeGreaterThan(0);
        result[0].Price.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ShouldRenderUpdatePricePageCorrectly()
    {
        // Arrange
        var component = RenderComponent<UpdatePrice>();

        // Act
        var title = component.Find("h3").TextContent.Trim(); 

        // Assert
        title.ShouldBe("Update prijzen");
    }
}
