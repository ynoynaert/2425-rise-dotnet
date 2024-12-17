using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Rise.Client.Machineries.Category;
using Rise.Client.Machineries.FakeServices;
using Blazorise.FluentValidation;
using FluentValidation;

namespace Rise.Client.Machineries.CategoryTests;

public class AddCategoryShould : TestContext
{
    public AddCategoryShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<CategoryDto.Create.Validator>();

        Services.AddScoped<ICategoryService, FakeCategoryService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }
    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<AddCategory>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Categorie toevoegen");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var component = RenderComponent<AddCategory>();

        // Act
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(2);
        component.Find(".invalid-feedback").TextContent.Contains("Naam is verplicht.");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenCodeIsEmpty()
    {
        // Arrange
        var component = RenderComponent<AddCategory>();

        // Act
        component.Find("#input-name").Input("ValidCategoryName");
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
        component.Find(".invalid-feedback").TextContent.Contains("Code is verplicht.");
    }

    [Fact]
    public void ShouldSubmitForm_WhenInputIsValid()
    {
        // Arrange
        var component = RenderComponent<AddCategory>();

        // Act
        component.Find("#input-name").Input("TestCategorie");
        component.Find("#input-code").Input("TC001");
        component.Find("button[type='submit']").Click();

        // Assert
        // Hier wordt alleen gecontroleerd of de submit-knop werd geactiveerd zonder validatiefouten
        var errorMessages = component.FindAll(".invalid-feedback");
        errorMessages.ShouldBeEmpty();
    }
}
