using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using AngleSharp.Dom;
using Blazorise.FluentValidation;
using FluentValidation;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.MachineryOption;

namespace Rise.Client.Machineries.MachineryOptionTests;

public class EditMachineryOptionsShould : TestContext
{
    public EditMachineryOptionsShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<MachineryOptionDto.Update.Validator>();

        Services.AddScoped<IMachineryService, FakeMachineryService>();
        Services.AddScoped<ICategoryService, FakeCategoryService>();
        Services.AddScoped<IMachineryTypeService, FakeMachineryTypeService>();
        Services.AddScoped<IMachineryOptionService, FakeMachineryOptionService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Opties Bewerken");
    }

    [Fact]
    public void ShouldLoadOptions_WhenOptionsAreAvailable()
    {
        // Arrange
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        var categoryName = component.Find("#category-name").GetInnerText();
        var name = component.Find("#option-name").GetInnerText();
        var price = component.Find("#option-price").GetInnerText();



        // Assert
        categoryName.ShouldBe("C1 - Category 1");
        name.ShouldBe("Code1 - Option 1");
        price.ShouldBe("Prijs: € 100");
    }

    [Fact]
    public void ShouldEnterEditMode_WhenEditButtonClicked()
    {
        // Arrange
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        component.Find("#edit-button").Click();

        // Assert
        component.Find("input").ShouldNotBeNull();

    }

    [Fact]
    public void ShouldSaveOption_WhenFormIsValid()
    {
        // Arrange  
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));
        component.FindAll("#edit-button").First().Click();

        // Act  
        var inputElement = component.Find("#price-input");
        inputElement.Input("150");
        var instance = component.Instance;

        component.Find("#save-button").Click();
        var machineryOptionService = component.Instance.MachineryOptionService;

        // Assert  
        machineryOptionService.GetMachineryOptionAsync(1)?.Result.Price.ShouldBe(150m);

    }

    [Fact]
    public void ShouldShowError_WhenPriceIsZero()
    {
        // Arrange  
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));
        component.FindAll("#edit-button").First().Click(); ;
        // Act  
        component.Find("input").Input("0");
        // Assert 
        component.Find(".invalid-feedback").TextContent.Contains("Prijs moet groter dan 0.01 zijn");
    }

    [Fact]
    public void ShouldCancelEditMode_WhenCancelButtonClicked()
    {
        // Arrange
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));
        component.Find("#edit-button").Click();

        // Act
        component.Find("#cancel-edit-button").Click();

        // Assert
        component.FindAll("#price-input").ShouldBeEmpty();
    }


    [Fact]
    public async Task ShouldRemoveOption_WhenDeleteButtonClickedAsync()
    {
        // Arrange
        var component = RenderComponent<EditMachineryOptions>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        component.FindAll("#delete-button").First().Click();
        component.Find("#yes-button").Click();

        // Assert
        var machineryOptionService = component.Instance.MachineryOptionService;
        var exception = await Assert.ThrowsAsync<Exception>(() => {
            machineryOptionService.GetMachineryOptionAsync(1);
            return Task.CompletedTask;
        });

        exception.Message.ShouldBe("MachineryOption not found");
    }
}