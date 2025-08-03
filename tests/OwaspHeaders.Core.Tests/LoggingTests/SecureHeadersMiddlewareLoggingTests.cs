using Microsoft.Extensions.Logging;
using Moq;

namespace OwaspHeaders.Core.Tests.LoggingTests;

public class SecureHeadersMiddlewareLoggingTests
{
    private readonly Mock<ILogger<SecureHeadersMiddleware>> _mockLogger;
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public SecureHeadersMiddlewareLoggingTests()
    {
        _mockLogger = new Mock<ILogger<SecureHeadersMiddleware>>();
        
        // Setup the logger to return true for all log levels by default
        _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        _onNext = _ => Task.CompletedTask;
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_WithNullConfig_LogsConfigurationError()
    {
        var middleware = new SecureHeadersMiddleware(_onNext, null, _mockLogger.Object);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        VerifyLogCalled(LogLevel.Error, 3001, "Configuration validation failed:");
    }

    [Fact]
    public async Task InvokeAsync_WithValidConfig_LogsMiddlewareInitialization()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Information, 1001, "SecureHeaders middleware initialized with");
        VerifyLogCalled(LogLevel.Information, 1004, "Generated");
    }

    [Fact]
    public async Task InvokeAsync_WithValidConfig_LogsHeadersAdded()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Information, 1002, "Added");
        VerifyLogCalled(LogLevel.Information, 1002, "security headers to response");
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
        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Information, 1003, "Request ignored due to URL exclusion rule:");
    }

    [Fact]
    public async Task InvokeAsync_WithHeadersEnabled_LogsIndividualHeaders()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Debug, 1005, "Added header");
    }

    [Fact]
    public async Task InvokeAsync_WithCOEPConfigurationIssue_LogsConfigurationWarning()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseCrossOriginEmbedderPolicy()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        VerifyLogCalled(LogLevel.Warning, 2003, "Cross-Origin-Embedder-Policy requires Cross-Origin-Resource-Policy to be enabled");
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

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Information, 9001, "SecureHeaders middleware initialized");
        VerifyLogCalled(LogLevel.Information, 9002, "Added");
    }

    [Fact]
    public async Task InvokeAsync_WithBaseEventId_UsesOffsetEventIds()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIdBase(5000)
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        VerifyLogCalled(LogLevel.Information, 5001, "SecureHeaders middleware initialized");
        VerifyLogCalled(LogLevel.Information, 5002, "Added");
        VerifyLogCalled(LogLevel.Information, 5004, "Generated");
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
        _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(false);

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Theory]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public async Task InvokeAsync_RespectsLogLevel(LogLevel enabledLevel)
    {
        _mockLogger.Setup(x => x.IsEnabled(enabledLevel)).Returns(true);
        _mockLogger.Setup(x => x.IsEnabled(It.Is<LogLevel>(l => l != enabledLevel))).Returns(false);

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        await middleware.InvokeAsync(_context);

        _mockLogger.Verify(x => x.IsEnabled(enabledLevel), Times.AtLeastOnce);
    }

    [Fact]
    public async Task InvokeAsync_WithCOEPConfigurationIssue_ChecksWarningLogLevel()
    {
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseCrossOriginEmbedderPolicy()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config, _mockLogger.Object);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        _mockLogger.Verify(x => x.IsEnabled(LogLevel.Warning), Times.AtLeastOnce);
    }

    [Fact]
    public async Task InvokeAsync_WithNullConfig_ChecksErrorLogLevel()
    {
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Error)).Returns(true);

        var middleware = new SecureHeadersMiddleware(_onNext, null, _mockLogger.Object);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        _mockLogger.Verify(x => x.IsEnabled(LogLevel.Error), Times.AtLeastOnce);
    }

    private void VerifyLogCalled(LogLevel logLevel, int eventId, string messageContains)
    {
        _mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.Is<EventId>(e => e.Id == eventId),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(messageContains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce,
            $"Expected log call with level {logLevel}, event ID {eventId}, and message containing '{messageContains}'");
    }
}