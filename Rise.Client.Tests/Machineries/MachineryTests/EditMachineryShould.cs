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
using Rise.Client.Machineries;
using Bunit;
using Microsoft.AspNetCore.Components;
using Rise.Client.Services;
using Rise.Client.Files;
using Blazorise.FluentValidation;
using FluentValidation;
using Serilog;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.Machinery;

namespace Rise.Client.Machineries.MachineryTests;

public class EditMachineryShould : TestContext
{
    public EditMachineryShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);

        Services.AddBlazorise()
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

        Services.AddValidatorsFromAssemblyContaining<MachineryDto.Update.Validator>();

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
        var component = RenderComponent<EditMachinery>();

        // Act
        var title = component.Find("h3").TextContent;

        // Assert
        title.ShouldBe("Machine bewerken");
    }

    [Fact]
    public void ShouldRenderFieldsWithData()
    {
        // Arrange
        var component = RenderComponent<EditMachinery>(parameters => parameters.Add(p => p.Id, 1));

        // Act
        var serialNumber = component.Find("#input-serialnumber").GetAttribute("value");
        var name = component.Find("#input-name").GetAttribute("value");
        var description = component.Find("#input-description").GetAttribute("value");
        var typeId = component.Find("#input-typeId").GetAttribute("value");

        // Assert
        serialNumber.ShouldBe("SN1");
        name.ShouldBe("Machinery 1");
        description.ShouldBe("Description 1");
        typeId.ShouldBe("1");

    }
}