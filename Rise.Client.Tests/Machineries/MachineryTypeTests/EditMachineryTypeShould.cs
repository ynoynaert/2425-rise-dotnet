using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Microsoft.AspNetCore.Components;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.MachineryType;
using Blazorise.FluentValidation;
using FluentValidation;

namespace Rise.Client.Machineries.MachineryTypeTests;

public class EditMachineryTypeShould : TestContext
{
    public EditMachineryTypeShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();


        Services.AddValidatorsFromAssemblyContaining<MachineryTypeDto.Update.Validator>();

        Services.AddScoped<IMachineryTypeService, FakeMachineryTypeService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<EditMachineryType>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Type bewerken");
    }

    [Fact]
    public void ShouldDisplayValidationErrorForEmptyName()
    {
        // Arrange
        var component = RenderComponent<EditMachineryType>(parameters => parameters.Add(p => p.Id, 1));
        component.Find("#input-name").Input("");

        // Act
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldSubmitFormWhenValidDataEntered()
    {
        var component = RenderComponent<EditMachineryType>(parameters => parameters.Add(p => p.Id, 1));

        var name = component.Find("#input-name");
        name.Input("Test");

        component.Find("button[type='submit']").Click();

        var navigationManager = Services.GetService<NavigationManager>();
        navigationManager!.Uri.ShouldContain("/machinetypes");
    }
}
