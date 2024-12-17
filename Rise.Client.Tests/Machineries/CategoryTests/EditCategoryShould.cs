using System.Threading.Tasks;
using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using System;
using System.Linq;
using Rise.Client.Machineries.Category;
using Rise.Client.Machineries.FakeServices;
using Blazorise.FluentValidation;
using FluentValidation;

namespace Rise.Client.Machineries.CategoryTests;
public class EditCategoryShould : TestContext
{
    public EditCategoryShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<CategoryDto.Update.Validator>();

        Services.AddScoped<ICategoryService, FakeCategoryService>();
        Services.AddScoped<IOptionService, FakeOptionService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<EditCategory>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Categorie bewerken");
    }

    [Fact]
    public void ShouldLoadCategoryDetails_WhenIdIsValid()
    {
        // Arrange
        var component = RenderComponent<EditCategory>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        var nameField = component.Find("#input-name").GetAttribute("value");
        var codeField = component.Find("#input-code").GetAttribute("value");

        // Assert
        nameField.ShouldBe("Category 1");
        codeField.ShouldBe("Code 1");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var component = RenderComponent<EditCategory>(parameters => parameters.Add(p => p.Id, 1));
        component.Find("#input-name").Input("");

        // Act
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
        component.Find(".invalid-feedback").TextContent.Contains("Naam is verplicht.");
    }

    [Fact]
    public void ShouldDisplayValidationError_WhenCodeIsEmpty()
    {
        // Arrange
        var component = RenderComponent<EditCategory>(parameters => parameters.Add(p => p.Id, 1));
        component.Find("#input-code").Input("");

        // Act
        component.Find("button[type='submit']").Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(1);
        component.Find(".invalid-feedback").TextContent.Contains("Code is verplicht.");
    }

    [Fact]
    public void ShouldSubmitForm_WhenInputIsValid()
    {
        // Arrange
        var component = RenderComponent<EditCategory>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        component.Find("#input-name").Input("UpdatedName");
        component.Find("#input-code").Input("UpdatedCode");
        component.Find("button[type='submit']").Click();

        // Controleer of de wijzigingen correct zijn door opnieuw te laden
        var nameField = component.Find("#input-name").GetAttribute("value");
        var codeField = component.Find("#input-code").GetAttribute("value");

        nameField.ShouldBe("UpdatedName");
        codeField.ShouldBe("UpdatedCode");
    }

}
