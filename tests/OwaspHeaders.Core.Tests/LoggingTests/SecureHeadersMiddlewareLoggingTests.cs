using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace OwaspHeaders.Core.Tests.LoggingTests;

public class SecureHeadersMiddlewareLoggingTests
{
    private readonly FakeLogger<SecureHeadersMiddleware> _logger;
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public SecureHeadersMiddlewareLoggingTests()
    {
        _logger = new FakeLogger<SecureHeadersMiddleware>();
        _onNext = _ => Task.CompletedTask;
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_WithNullConfig_LogsConfigurationError()
    {
        var middleware = new SecureHeadersMiddleware(_onNext, null, _logger);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        AssertLogged(LogLevel.Error, 3001, "Configuration validation failed:");
    }

    [Fact]
    public async Task InvokeAsync_WithValidConfig_LogsMiddlewareInitialization()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Information, 1001, "SecureHeaders middleware initialized with");
        AssertLogged(LogLevel.Information, 1004, "Generated");
    }

    [Fact]
    public async Task InvokeAsync_WithValidConfig_LogsHeadersAdded()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Information, 1002, "Added");
        AssertLogged(LogLevel.Information, 1002, "security headers to response");
    }

    [Fact]
    public async Task InvokeAsync_WithIgnoredUrl_LogsRequestIgnored()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .SetUrlsToIgnore(["/ignore"])
            .Build();

        _context.Request.Path = "/ignore";
        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Information, 1003, "Request ignored due to URL exclusion rule:");
    }

    [Fact]
    public async Task InvokeAsync_WithHeadersEnabled_LogsIndividualHeaders()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Debug, 1005, "Added header");
    }

    [Fact]
    public async Task InvokeAsync_WithCOEPConfigurationIssue_LogsConfigurationWarning()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseCrossOriginEmbedderPolicy()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        AssertLogged(LogLevel.Warning, 2003, "Cross-Origin-Embedder-Policy requires Cross-Origin-Resource-Policy to be enabled");
    }

    [Fact]
    public async Task InvokeAsync_WithCustomEventIds_UsesCustomEventIds()
    {
        var customConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(9001, "CustomInit"),
            HeadersAdded = new EventId(9002, "CustomHeaders")
        };

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIds(customConfig)
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Information, 9001, "SecureHeaders middleware initialized");
        AssertLogged(LogLevel.Information, 9002, "Added");
    }

    [Fact]
    public async Task InvokeAsync_WithBaseEventId_UsesOffsetEventIds()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIdBase(5000)
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        AssertLogged(LogLevel.Information, 5001, "SecureHeaders middleware initialized");
        AssertLogged(LogLevel.Information, 5002, "Added");
        AssertLogged(LogLevel.Information, 5004, "Generated");
    }

    [Fact]
    public async Task InvokeAsync_WithNullLogger_DoesNotThrow()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, logger: null);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.Null(exception);
        Assert.Contains("Strict-Transport-Security", _context.Response.Headers.Keys);
    }

    [Fact]
    public async Task InvokeAsync_WithLoggerDisabled_DoesNotLog()
    {
        foreach (var level in AllLogLevels)
        {
            _logger.ControlLevel(level, false);
        }

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        Assert.Empty(_logger.Collector.GetSnapshot());
    }

    [Theory]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public async Task InvokeAsync_RespectsLogLevel(LogLevel enabledLevel)
    {
        foreach (var level in AllLogLevels)
        {
            _logger.ControlLevel(level, level == enabledLevel);
        }

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        await middleware.InvokeAsync(_context);

        var snapshot = _logger.Collector.GetSnapshot();
        Assert.NotEmpty(snapshot);
        Assert.All(snapshot, r => Assert.Equal(enabledLevel, r.Level));
    }

    [Fact]
    public async Task InvokeAsync_WithFlagButNoMatchingConfig_LogsConsolidatedConfigurationError()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseReferrerPolicy()
            .Build();

        config.UseCacheControl = true;
        config.UseHsts = true;

        var middleware = new SecureHeadersMiddleware(_onNext, config, _logger);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        AssertLogged(LogLevel.Error, 3001, nameof(SecureHeadersMiddlewareConfiguration.UseCacheControl));
        AssertLogged(LogLevel.Error, 3001, nameof(SecureHeadersMiddlewareConfiguration.UseHsts));
    }

    private static readonly LogLevel[] AllLogLevels =
    [
        LogLevel.Trace, LogLevel.Debug, LogLevel.Information,
        LogLevel.Warning, LogLevel.Error, LogLevel.Critical
    ];

    private void AssertLogged(LogLevel level, int eventId, string messageContains)
    {
        Assert.Contains(_logger.Collector.GetSnapshot(),
            r => r.Level == level && r.Id.Id == eventId && r.Message.Contains(messageContains));
    }
}
