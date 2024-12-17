using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using Blazorise.FluentValidation;
using FluentValidation;
using Rise.Client.Files;
using Serilog;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.Machinery;

namespace Rise.Client.Machineries.MachineryTests;

public class AddMachineryShould : TestContext
{
    public AddMachineryShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<MachineryDto.Create.Validator>();

        Services.AddScoped<IMachineryService, FakeMachineryService>();
        Services.AddScoped<ICategoryService, FakeCategoryService>();
        Services.AddScoped<IMachineryTypeService, FakeMachineryTypeService>();
        Services.AddScoped<IStorageService, FakeStorageService>();
        Services.AddSingleton<IIdGenerator, IdGenerator>();
    }

    [Fact]
    public void ShouldRenderCorrectly()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Machine toevoegen");
    }


    [Fact]
    public void ShouldDisplayValidationErrorsForEmptyRequiredFields()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        // Act
        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(5);
    }

    [Fact]
    public void ShouldDisplayValidationErrorsForEmptySerialnumber()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        var nameField = component.Find("#input-name");
        var descriptionField = component.Find("#input-description");
        var typeSelect = component.Find("#input-typeId");

        // Act
        nameField.Input("New Machine");
        descriptionField.Input("A test machine");
        typeSelect.Change("1");

        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(2);
        component.Find(".invalid-feedback").TextContent.Contains("Serienummer moet ingevuld zijn.");
    }

    [Fact]
    public void ShouldDisplayValidationErrorsForEmptyName()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        var serialNumberField = component.Find("#input-serialnumber");
        var descriptionField = component.Find("#input-description");
        var typeSelect = component.Find("#input-typeId");

        // Act
        serialNumberField.Input("SN12345");
        descriptionField.Input("A test machine");
        typeSelect.Change("1");

        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        component.FindAll(".invalid-feedback").Count.ShouldBe(2);
        component.Find(".invalid-feedback").TextContent.Contains("Naam moet ingevuld zijn.");
    }

    [Fact]
    public void ShouldDisplayValidationErrorsForEmptyDescription()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        var serialNumberField = component.Find("#input-serialnumber");
        var nameField = component.Find("#input-name");
        var typeSelect = component.Find("#input-typeId");

        // Act
        serialNumberField.Input("SN12345");
        nameField.Input("New Machine");
        typeSelect.Change("1");

        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        var navigationManager = Services.GetService<NavigationManager>();
        component.Find(".invalid-feedback").TextContent.Contains("Beschrijving moet ingevuld zijn.");
    }

    [Fact]
    public void ShouldDisplayValidationErrorsForEmptyTypeId()
    {
        // Arrange
        var component = RenderComponent<AddMachinery>();

        var serialNumberField = component.Find("#input-serialnumber");
        var nameField = component.Find("#input-name");
        var descriptionField = component.Find("#input-description");

        // Act
        serialNumberField.Input("SN12345");
        nameField.Input("New Machine");
        descriptionField.Input("A test machine");

        var button = component.Find("button[type='submit']");
        button.Click();

        // Assert
        var navigationManager = Services.GetService<NavigationManager>();
        component.Find(".invalid-feedback").TextContent.Contains("Type moet ingevuld zijn.");
    }
}