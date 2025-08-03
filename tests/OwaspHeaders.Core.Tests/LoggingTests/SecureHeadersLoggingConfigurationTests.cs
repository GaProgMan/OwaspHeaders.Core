using Microsoft.Extensions.Logging;

namespace OwaspHeaders.Core.Tests.LoggingTests;

public class SecureHeadersLoggingConfigurationTests
{
    [Fact]
    public void Constructor_SetsDefaultEventIds()
    {
        var config = new SecureHeadersLoggingConfiguration();

        Assert.Equal(1001, config.MiddlewareInitialized.Id);
        Assert.Equal(1002, config.HeadersAdded.Id);
        Assert.Equal(1003, config.RequestIgnored.Id);
        Assert.Equal(1004, config.HeadersGenerated.Id);
        Assert.Equal(1005, config.HeaderAdded.Id);
        Assert.Equal(2001, config.HeaderAdditionFailed.Id);
        Assert.Equal(2002, config.HeaderRemovalFailed.Id);
        Assert.Equal(2003, config.ConfigurationIssue.Id);
        Assert.Equal(3001, config.ConfigurationError.Id);
        Assert.Equal(3002, config.MiddlewareException.Id);
    }

    [Fact]
    public void Constructor_SetsEventIdNames()
    {
        var config = new SecureHeadersLoggingConfiguration();

        Assert.Equal(nameof(config.MiddlewareInitialized), config.MiddlewareInitialized.Name);
        Assert.Equal(nameof(config.HeadersAdded), config.HeadersAdded.Name);
        Assert.Equal(nameof(config.RequestIgnored), config.RequestIgnored.Name);
        Assert.Equal(nameof(config.HeadersGenerated), config.HeadersGenerated.Name);
        Assert.Equal(nameof(config.HeaderAdded), config.HeaderAdded.Name);
        Assert.Equal(nameof(config.HeaderAdditionFailed), config.HeaderAdditionFailed.Name);
        Assert.Equal(nameof(config.HeaderRemovalFailed), config.HeaderRemovalFailed.Name);
        Assert.Equal(nameof(config.ConfigurationIssue), config.ConfigurationIssue.Name);
        Assert.Equal(nameof(config.ConfigurationError), config.ConfigurationError.Name);
        Assert.Equal(nameof(config.MiddlewareException), config.MiddlewareException.Name);
    }

    [Theory]
    [InlineData(5000)]
    [InlineData(10000)]
    [InlineData(0)]
    public void CreateWithBaseEventId_OffsetsEventIds(int baseEventId)
    {
        var config = SecureHeadersLoggingConfiguration.CreateWithBaseEventId(baseEventId);

        Assert.Equal(baseEventId + 1, config.MiddlewareInitialized.Id);
        Assert.Equal(baseEventId + 2, config.HeadersAdded.Id);
        Assert.Equal(baseEventId + 3, config.RequestIgnored.Id);
        Assert.Equal(baseEventId + 4, config.HeadersGenerated.Id);
        Assert.Equal(baseEventId + 5, config.HeaderAdded.Id);
        Assert.Equal(baseEventId + 101, config.HeaderAdditionFailed.Id);
        Assert.Equal(baseEventId + 102, config.HeaderRemovalFailed.Id);
        Assert.Equal(baseEventId + 103, config.ConfigurationIssue.Id);
        Assert.Equal(baseEventId + 201, config.ConfigurationError.Id);
        Assert.Equal(baseEventId + 202, config.MiddlewareException.Id);
    }

    [Fact]
    public void CreateWithBaseEventId_PreservesEventIdNames()
    {
        var config = SecureHeadersLoggingConfiguration.CreateWithBaseEventId(5000);

        Assert.Equal("MiddlewareInitialized", config.MiddlewareInitialized.Name);
        Assert.Equal("HeadersAdded", config.HeadersAdded.Name);
        Assert.Equal("RequestIgnored", config.RequestIgnored.Name);
        Assert.Equal("HeadersGenerated", config.HeadersGenerated.Name);
        Assert.Equal("HeaderAdded", config.HeaderAdded.Name);
        Assert.Equal("HeaderAdditionFailed", config.HeaderAdditionFailed.Name);
        Assert.Equal("HeaderRemovalFailed", config.HeaderRemovalFailed.Name);
        Assert.Equal("ConfigurationIssue", config.ConfigurationIssue.Name);
        Assert.Equal("ConfigurationError", config.ConfigurationError.Name);
        Assert.Equal("MiddlewareException", config.MiddlewareException.Name);
    }

    [Fact]
    public void EventIds_CanBeCustomized()
    {
        var config = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(9001, "CustomInit"),
            HeadersAdded = new EventId(9002, "CustomHeaders")
        };

        Assert.Equal(9001, config.MiddlewareInitialized.Id);
        Assert.Equal("CustomInit", config.MiddlewareInitialized.Name);
        Assert.Equal(9002, config.HeadersAdded.Id);
        Assert.Equal("CustomHeaders", config.HeadersAdded.Name);
    }
}
