using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Persistence.Triggers;
using Rise.Server.Middleware;
using Rise.Services.Machineries;
using Rise.Shared.Machineries;
using Rise.Services.Translations;
using Rise.Shared.Translations;
using Rise.Shared.Quotes;
using Rise.Services.Quotes;
using Rise.Shared.Customers;
using Rise.Services.Customers;
using Rise.Services.Files;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Auth0Net.DependencyInjection;
using Microsoft.OpenApi.Models;
using Rise.Shared.Users;
using Rise.Services.Users;
using Auth0.ManagementApi;
using Rise.Shared.Orders;
using Rise.Services.Orders;
using Rise.Services.Inquiries;
using Rise.Shared.Inquiries;
using Rise.Shared.Locations;
using Rise.Services.Locations;
using Rise.Persistence.Migrations;
using Rise.Shared.Documents;
using Rise.Services.Documents;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateLogger();

try
{
    Log.Information("Launching web application...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Configuration.AddUserSecrets<Program>();

    builder.Services

    .AddValidatorsFromAssemblyContaining<MachineryDto.Create.Validator>()
    .AddFluentValidationAutoValidation();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/oauth/token"),
                    AuthorizationUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/authorize?audience={builder.Configuration["Auth0:Audience"]}"),
                }
            }
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new string[] { "openid" }
        }
        });
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

    builder.Services.AddAuth0AuthenticationClient(config =>
    {
        config.Domain = builder.Configuration["Auth0:Authority"]!;
        config.ClientId = builder.Configuration["Auth0:M2MClientId"];
        config.ClientSecret = builder.Configuration["Auth0:M2MClientSecret"];
    });
    builder.Services.AddAuth0ManagementClient().AddManagementAccessToken();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>());
    });

    builder.Services.AddScoped<IMachineryService, MachineryService>();
    builder.Services.AddScoped<IMachineryOptionService, MachineryOptionService>();
    builder.Services.AddScoped<IOptionService, OptionService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IMachineryTypeService, MachineryTypeService>();
    builder.Services.AddScoped<ITranslationService, TranslationService>();
    builder.Services.AddScoped<IQuoteService, QuoteService>();
    builder.Services.AddScoped<IQuoteOptionService, QuoteOptionService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IStorageService, BlobStorageService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<ILocationService, LocationService>();
    builder.Services.AddScoped<ITradedMachineryService, TradedMachineryService>();
    builder.Services.AddScoped<IDocumentService, DocumentService>();
    builder.Services.AddScoped<IInquiryService, InquiryService>();
    builder.Services.AddScoped<IInquiryOptionService, InquiryOptionService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
            options.OAuthClientId(builder.Configuration["Auth0:BlazorClientId"]);
            options.OAuthClientSecret(builder.Configuration["Auth0:BlazorClientSecret"]);
        });
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers().RequireAuthorization();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseRouting();

    app.MapControllers();
    app.MapFallbackToFile("index.html");

    using (var scope = app.Services.CreateScope())
    { // Require a DbContext from the service provider and seed the database.
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Seeder seeder = new(dbContext);
        seeder.Seed();
    }

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}