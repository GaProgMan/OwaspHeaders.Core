using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;

namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// Regression tests for https://github.com/GaProgMan/OwaspHeaders.Core/issues/261
///
/// <see cref="CacheControl.BuildHeaderValue"/> used to return as soon as it found the first
/// set flag, so the flags could not be combined (e.g. <c>noCache: true</c> silently dropped
/// <c>no-store</c>). The default value also lacked <c>no-cache</c>, which caused ASP.NET Core's
/// antiforgery system to override the header and log a warning on every response that
/// generated an antiforgery token.
/// </summary>
public class Issue261RegressionTests
{
    // Antiforgery logs ResponseCacheHeadersOverridenToNoCache with this event ID when it
    // overrides a Cache-Control header lacking either no-cache or no-store
    private const int AntiforgeryCacheHeaderOverriddenEventId = 8;
    private const string FormUrl = "/form";

    [Theory]
    [InlineData(false, 0, true, true, false, "no-cache, no-store, max-age=0")]
    [InlineData(true, 0, true, true, false, "private, no-cache, no-store, max-age=0")]
    [InlineData(false, 0, true, true, true, "no-cache, no-store, max-age=0, must-revalidate")]
    [InlineData(false, 0, false, true, false, "no-store, max-age=0")]
    [InlineData(false, 0, true, false, false, "no-cache, max-age=0")]
    [InlineData(false, 60, false, false, false, "max-age=60")]
    [InlineData(true, 60, false, false, true, "private, max-age=60, must-revalidate")]
    [InlineData(true, 30, true, true, true, "private, no-cache, no-store, max-age=30, must-revalidate")]
    public void BuildHeaderValue_IncludesEveryRequestedDirective(bool @private, int maxAge, bool noCache,
        bool noStore, bool mustRevalidate, string expected)
    {
        var cacheControl = new CacheControl(@private, maxAge, noCache, noStore, mustRevalidate);

        Assert.Equal(expected, cacheControl.BuildHeaderValue());
    }

    [Fact]
    public void UseCacheControl_NoCache_KeepsNoStore()
    {
        // the "obvious fix" from issue #261, which used to drop no-store entirely
        var config = new SecureHeadersBuilder()
            .UseCacheControl(noCache: true)
            .Build();

        Assert.True(config.UseCacheControl);
        var directives = Directives(config.CacheControl.BuildHeaderValue());

        Assert.Contains("no-cache", directives);
        Assert.Contains("no-store", directives);
    }

    [Fact]
    public void UseCacheControl_NoStoreFalse_HasNoEmptyDirective()
    {
        var config = new SecureHeadersBuilder()
            .UseCacheControl(noStore: false)
            .Build();

        Assert.True(config.UseCacheControl);
        var headerValue = config.CacheControl.BuildHeaderValue();

        Assert.DoesNotContain(headerValue.Split(','), d => string.IsNullOrWhiteSpace(d));
    }

    [Fact]
    public void BuildHeaderValue_NegativeMaxAge_IsSentAsZero()
    {
        var cacheControl = new CacheControl(false, maxAge: -1);

        Assert.Equal("no-cache, no-store, max-age=0", cacheControl.BuildHeaderValue());
    }

    [Fact]
    public void Deserialize_MissingNoCache_UsesNewDefault()
    {
        var cacheControl = JsonSerializer.Deserialize<CacheControl>("{\"Private\":false,\"MaxAge\":0}");

        Assert.NotNull(cacheControl);
        Assert.True(cacheControl.NoCache);
        Assert.True(cacheControl.NoStore);
    }

    [Fact]
    public void Defaults_IncludeNoCacheAndNoStore()
    {
        var builderConfig = new SecureHeadersBuilder().UseCacheControl().Build();
        var defaultConfig = new SecureHeadersBuilder().UseRecommendedDefaults().Build();

        Assert.True(builderConfig.UseCacheControl);
        Assert.True(defaultConfig.UseCacheControl);
        var builderDirectives = Directives(builderConfig.CacheControl.BuildHeaderValue());
        var defaultConfigDirectives = Directives(defaultConfig.CacheControl.BuildHeaderValue());

        Assert.Contains("no-cache", builderDirectives);
        Assert.Contains("no-store", builderDirectives);
        Assert.Contains("no-cache", defaultConfigDirectives);
        Assert.Contains("no-store", defaultConfigDirectives);
    }

    [Fact]
    public async Task DefaultConfiguration_WithAntiforgery_IsNotOverriddenOrWarned()
    {
        var logs = new CapturingLoggerProvider();

        // configure: null uses the parameterless app.UseSecureHeadersMiddleware()
        var context = await GetFormResponse(configure: null, logs);

        Assert.Equal("no-cache, no-store, max-age=0", context.Response.Headers[Constants.CacheControlHeaderName]);
        Assert.DoesNotContain(logs.Entries, e => e.EventId.Id == AntiforgeryCacheHeaderOverriddenEventId
                                                 && e.Category.StartsWith("Microsoft.AspNetCore.Antiforgery"));
    }

    [Fact]
    public async Task WithoutNoCache_WithAntiforgery_IsOverriddenAndWarned()
    {
        // proves that the test above would detect the warning from issue #261
        var logs = new CapturingLoggerProvider();

        await GetFormResponse(opt => opt.UseCacheControl(noCache: false), logs);

        Assert.Contains(logs.Entries, e => e.EventId.Id == AntiforgeryCacheHeaderOverriddenEventId
                                           && e.Category.StartsWith("Microsoft.AspNetCore.Antiforgery"));
    }

    private static string[] Directives(string headerValue) =>
        headerValue.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private static async Task<HttpContext> GetFormResponse(Action<SecureHeadersBuilder>? configure,
        CapturingLoggerProvider logs)
    {
        using var host = await new HostBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.AddProvider(logs);
            })
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddAntiforgery();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        if (configure is null)
                        {
                            app.UseSecureHeadersMiddleware();
                        }
                        else
                        {
                            app.UseSecureHeadersMiddleware(configure);
                        }

                        app.UseEndpoints(endpoints =>
                        {
                            // what any Razor page containing <form method="post"> does
                            endpoints.MapGet(FormUrl, (HttpContext context, IAntiforgery antiforgery) =>
                            {
                                antiforgery.GetAndStoreTokens(context);
                                return TypedResults.Text("Hello Tests");
                            });
                        });
                    });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://example.com/");

        return await testServer.SendAsync(c =>
        {
            c.Request.Path = FormUrl;
            c.Request.Method = HttpMethods.Get;
        }, TestContext.Current.CancellationToken);
    }

    private sealed record LogEntry(string Category, EventId EventId, string Message);

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public ConcurrentBag<LogEntry> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, Entries);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger : ILogger
        {
            private readonly string _category;
            private readonly ConcurrentBag<LogEntry> _entries;

            public CapturingLogger(string category, ConcurrentBag<LogEntry> entries)
            {
                _category = category;
                _entries = entries;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                _entries.Add(new LogEntry(_category, eventId, formatter(state, exception)));
            }
        }
    }
}
