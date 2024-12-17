using Blazorise.Utilities;
using Blazorise;
using Rise.Shared.Machineries;
using Xunit.Abstractions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Shouldly;
using Rise.Client.Machineries.FakeServices;
using Rise.Client.Machineries.MachineryOption;
using Blazorise.FluentValidation;
using FluentValidation;

namespace Rise.Client.Machineries.MachineryOptionTests;

public class AddMachineryOptionShould : TestContext
{

	public AddMachineryOptionShould(ITestOutputHelper outputHelper)
	{
		
		Services.AddXunitLogger(outputHelper);

		Services.AddBlazorise()
				.AddBootstrap5Providers()
				.AddFontAwesomeIcons()
                .AddBlazoriseFluentValidation();

		Services.AddValidatorsFromAssemblyContaining<MachineryOptionDto.Create.Validator>();

		Services.AddScoped<IMachineryService, FakeMachineryService>();
		Services.AddScoped<IMachineryOptionService, FakeMachineryOptionService>();
		Services.AddScoped<IOptionService, FakeOptionService>();
		Services.AddSingleton<IIdGenerator, IdGenerator>();

    JSInterop.Setup<object>("blazoredTypeahead.addKeyDownEventListener", _ => true);
    
    }

  [Fact]
	public void ShouldRenderCorrectly()
	{
		// Arrange
		var component = RenderComponent<AddMachineryOption>(parameters => parameters.Add(p => p.MachineryId, 1));

		// Act
		var title = component.Find("h3").TextContent;

		// Assert
		title.ShouldBe("Nieuwe optie toevoegen");
	}

	[Fact]
	public void ShouldDisplayValidationErrorsForEmptyRequiredFields()
	{
		// Arrange
		var component = RenderComponent<AddMachineryOption>(parameters => parameters.Add(p => p.MachineryId, 1));

		// Act
		component.Find("button[type='submit']").Click();

		// Assert
		component.FindAll(".invalid-feedback").Count.ShouldBe(1);
	}

}


