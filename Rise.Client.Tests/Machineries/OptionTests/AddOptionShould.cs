using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Blazorise.FluentValidation;
using FluentValidation;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.Option;

namespace Rise.Client.Machineries.OptionTests;

public class AddOptionShould : TestContext
{
    public AddOptionShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<OptionDto.Create.Validator>();

        Services.AddScoped<IOptionService, FakeOptionService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<AddOption>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Optie toevoegen");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenCodeIsEmpty()
    {
        // Arrange
        var component = RenderComponent<AddOption>();

        // Act
        component.Find("#input-name").Input("Nieuwe Optie");
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
        component.Find(".invalid-feedback").TextContent.Contains("Code is verplicht.");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var component = RenderComponent<AddOption>();

        // Act
        component.Find("#input-code").Input("o1");
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
        component.Find(".invalid-feedback").TextContent.Contains("Naam is verplicht.");
    }

    [Fact]
    public void ShouldSubmitForm_WhenInputIsValid()
    {
        // Arrange
        var component = RenderComponent<AddOption>();

        // Act
        component.Find("#input-name").Input("Nieuwe Optie");
        component.Find("#input-code").Input("o1");
        component.Find("button[type='submit']").Click();

        // Assert
        var errorMessages = component.FindAll(".invalid-feedback");
        errorMessages.ShouldBeEmpty();
    }
}