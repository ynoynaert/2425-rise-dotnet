﻿using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Xunit.Abstractions;

namespace Rise.Client;

/// <summary>
/// Logging for xUnit helper class, taken from: https://bunit.dev/docs/misc-test-tips.html
/// </summary>
public static class ServiceCollectionLoggingExtensions
{
    public static IServiceCollection AddXunitLogger(this IServiceCollection services, ITestOutputHelper outputHelper)
    {
        var serilogLogger = new LoggerConfiguration()
          .MinimumLevel.Verbose()
          .WriteTo.TestOutput(
            testOutputHelper: outputHelper,
            formatter: new ExpressionTemplate("[{UtcDateTime(@t):mm:ss.ffffff} | {@l:u3} | {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)} | {Coalesce(EventId.Name, '<none>')}] {@m}\n{@x}"),
            restrictedToMinimumLevel: LogEventLevel.Verbose)
          .CreateLogger();

        services.AddSingleton(_ => new LoggerFactory().AddSerilog(serilogLogger, dispose: true));
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        return services;
    }
}