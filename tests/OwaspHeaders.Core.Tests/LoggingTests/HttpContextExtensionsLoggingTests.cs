using Microsoft.Extensions.Logging;
using Moq;

namespace OwaspHeaders.Core.Tests.LoggingTests;

public class HttpContextExtensionsLoggingTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly DefaultHttpContext _context;

    public HttpContextExtensionsLoggingTests()
    {
        _mockLogger = new Mock<ILogger>();
        _context = new DefaultHttpContext();
    }

    [Fact]
    public void TryAddHeader_WithValidHeader_ReturnsTrue()
    {
        var result = _context.TryAddHeader("Test-Header", "test-value");

        Assert.True(result);
        Assert.Equal("test-value", _context.Response.Headers["Test-Header"]);
    }

    [Fact]
    public void TryAddHeader_WithExistingHeader_ReturnsTrue()
    {
        _context.Response.Headers.Append("Existing-Header", "existing-value");

        var result = _context.TryAddHeader("Existing-Header", "new-value");

        Assert.True(result);
        Assert.Equal("existing-value", _context.Response.Headers["Existing-Header"]);
    }

    [Fact]
    public void TryAddHeader_WithLogger_DoesNotLogOnSuccess()
    {
        var eventId = new EventId(2001, "TestEvent");

        var result = _context.TryAddHeader("Test-Header", "test-value", _mockLogger.Object, eventId);

        Assert.True(result);
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void TryAddHeader_WithNullLogger_DoesNotThrow()
    {
        var eventId = new EventId(2001, "TestEvent");

        var exception = Record.Exception(() =>
            _context.TryAddHeader("Test-Header", "test-value", null, eventId));

        Assert.Null(exception);
        Assert.Equal("test-value", _context.Response.Headers["Test-Header"]);
    }

    [Fact]
    public void TryAddHeader_WithoutEventId_DoesNotLog()
    {
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);

        var result = _context.TryAddHeader("Test-Header", "test-value", _mockLogger.Object);

        Assert.True(result);
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void TryAddHeader_WithLoggerDisabled_DoesNotLog()
    {
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(false);
        var eventId = new EventId(2001, "TestEvent");

        var result = _context.TryAddHeader("Test-Header", "test-value", _mockLogger.Object, eventId);

        Assert.True(result);
        // IsEnabled is only called when there's an exception, which doesn't happen on successful addition
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void TryRemoveHeader_WithValidHeader_ReturnsTrue()
    {
        _context.Response.Headers.Append("Test-Header", "test-value");

        var result = _context.TryRemoveHeader("Test-Header");

        Assert.True(result);
        Assert.False(_context.Response.Headers.ContainsKey("Test-Header"));
    }

    [Fact]
    public void TryRemoveHeader_WithNonExistentHeader_ReturnsTrue()
    {
        var result = _context.TryRemoveHeader("Non-Existent-Header");

        Assert.True(result);
    }

    [Fact]
    public void TryRemoveHeader_WithLogger_DoesNotLogOnSuccess()
    {
        _context.Response.Headers.Append("Test-Header", "test-value");
        var eventId = new EventId(2002, "TestEvent");

        var result = _context.TryRemoveHeader("Test-Header", _mockLogger.Object, eventId);

        Assert.True(result);
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void TryRemoveHeader_WithNullLogger_DoesNotThrow()
    {
        _context.Response.Headers.Append("Test-Header", "test-value");
        var eventId = new EventId(2002, "TestEvent");

        var exception = Record.Exception(() =>
            _context.TryRemoveHeader("Test-Header", null, eventId));

        Assert.Null(exception);
        Assert.False(_context.Response.Headers.ContainsKey("Test-Header"));
    }

    [Fact]
    public void TryRemoveHeader_WithoutEventId_DoesNotLog()
    {
        _context.Response.Headers.Append("Test-Header", "test-value");
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);

        var result = _context.TryRemoveHeader("Test-Header", _mockLogger.Object);

        Assert.True(result);
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void TryRemoveHeader_WithLoggerDisabled_DoesNotLog()
    {
        _context.Response.Headers.Append("Test-Header", "test-value");
        _mockLogger.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(false);
        var eventId = new EventId(2002, "TestEvent");

        var result = _context.TryRemoveHeader("Test-Header", _mockLogger.Object, eventId);

        Assert.True(result);
        // IsEnabled is only called when there's an exception, which doesn't happen on successful removal
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [Fact]
    public void Extension_Methods_MaintainBackwardCompatibility()
    {
        _context.Response.Headers.Append("Existing-Header", "value");

        var addResult = _context.TryAddHeader("New-Header", "new-value");
        var removeResult = _context.TryRemoveHeader("Existing-Header");

        Assert.True(addResult);
        Assert.True(removeResult);
        Assert.Equal("new-value", _context.Response.Headers["New-Header"]);
        Assert.False(_context.Response.Headers.ContainsKey("Existing-Header"));
    }

    [Theory]
    [InlineData("Valid-Header", "valid-value")]
    [InlineData("X-Custom-Header", "custom-value")]
    [InlineData("Content-Security-Policy", "default-src 'self'")]
    public void TryAddHeader_WithVariousHeaders_Succeeds(string headerName, string headerValue)
    {
        var result = _context.TryAddHeader(headerName, headerValue);

        Assert.True(result);
        Assert.Equal(headerValue, _context.Response.Headers[headerName]);
    }

    [Theory]
    [InlineData("Header-To-Remove")]
    [InlineData("X-Custom-Header")]
    [InlineData("Content-Security-Policy")]
    public void TryRemoveHeader_WithVariousHeaders_Succeeds(string headerName)
    {
        _context.Response.Headers.Append(headerName, "some-value");

        var result = _context.TryRemoveHeader(headerName);

        Assert.True(result);
        Assert.False(_context.Response.Headers.ContainsKey(headerName));
    }
}
