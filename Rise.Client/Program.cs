using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using FluentValidation;
using Rise.Shared.Machineries;
using Rise.Shared.Translations;
using Rise.Shared.Quotes;
using Rise.Client.Machineries.Services;
using Rise.Client.Translations;
using Rise.Client.Quotes;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Client.Auth;
using Rise.Client.Services;
using Rise.Client.Files;
using Rise.Client.SalesPeople;
using Rise.Shared.Users;
using Rise.Shared.Orders;
using Rise.Client.Orders;
using Rise.Client.Inquiries;
using Rise.Shared.Inquiries;
using Rise.Shared.Locations;
using Rise.Client.Locations;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// auth0
builder.Services.AddHttpClient("AuthenticatedClient.ServerAPI",
        client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
       .CreateClient("AuthenticatedClient.ServerAPI"));

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


builder.Services.AddTransient<CleanErrorHandler>();

builder.Services.AddHttpClient<IStorageService, AzureBlobStorageService>();

builder.Services.AddHttpClient<IMachineryService, MachineryService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
	client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IMachineryTypeService, MachineryTypeService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IMachineryOptionService, MachineryOptionService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IOptionService, OptionService>(client =>
{
	client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ILocationService, LocationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddSingleton<MachineryQueryService>();

builder.Services.AddSingleton<TranslationQueryService>();

builder.Services.AddSingleton<CategoryQueryService>();

builder.Services.AddSingleton<QuoteQueryService>();

builder.Services.AddSingleton<UserQueryService>();

builder.Services.AddSingleton<OrderQueryService>();

builder.Services.AddSingleton<UnacceptedTranslationQueryService>();

builder.Services.AddHttpClient<ITranslationService, TranslationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddHttpClient<IQuoteService, QuoteService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IQuoteOptionService, QuoteOptionService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddHttpClient<ITradedMachineryService, TradedMachineryService>(client =>
{
	client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IInquiryService, InquiryService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IInquiryOptionService, InquiryOptionsService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
}).AddHttpMessageHandler<CleanErrorHandler>().AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    .AddBlazoriseFluentValidation();

builder.Services.AddValidatorsFromAssembly( typeof( App ).Assembly )
    .AddValidatorsFromAssemblyContaining<MachineryDto.Create.Validator>();
await builder.Build().RunAsync();
