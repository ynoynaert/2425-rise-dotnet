using Blazorise.Utilities;
using Blazorise;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Microsoft.AspNetCore.Components;
using System.Linq;
using Blazorise.FluentValidation;
using FluentValidation;
using Serilog;

using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.MachineryType;
using Rise.Shared.Machineries;

namespace Rise.Client.Machineries.MachineryTypeTests;

public class AddMachineryTypeShould : TestContext
{
    public AddMachineryTypeShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<MachineryTypeDto.Create.Validator>();

        Services.AddScoped<IMachineryTypeService, FakeMachineryTypeService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<AddMachineryType>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Type toevoegen");
    }

    [Fact]
    public void ShouldDisplayValidationErrorForEmptyName()
    {
        // Arrange
        var component = RenderComponent<AddMachineryType>();

        // Act
        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldSubmitFormWhenValidDataEntered()
    {
        var component = RenderComponent<AddMachineryType>();

        var name = component.Find("#input-name");
        name.Input("Test");

        var button = component.Find("button[type='submit']");
        button.Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager!.Uri.ShouldContain("/machinetypes");
    }
}
