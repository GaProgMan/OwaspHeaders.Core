using Microsoft.Extensions.Logging;

namespace OwaspHeaders.Core.Tests.LoggingTests;

public class SecureHeadersBuilderLoggingTests
{
    [Fact]
    public void WithLoggingEventIds_SetsCustomEventIds()
    {
        var customLoggingConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(9001, "CustomInit"),
            HeadersAdded = new EventId(9002, "CustomHeaders"),
            ConfigurationError = new EventId(9003, "CustomError")
        };

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIds(customLoggingConfig)
            .Build();

        Assert.Equal(9001, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal("CustomInit", config.LoggingConfiguration.MiddlewareInitialized.Name);
        Assert.Equal(9002, config.LoggingConfiguration.HeadersAdded.Id);
        Assert.Equal("CustomHeaders", config.LoggingConfiguration.HeadersAdded.Name);
        Assert.Equal(9003, config.LoggingConfiguration.ConfigurationError.Id);
        Assert.Equal("CustomError", config.LoggingConfiguration.ConfigurationError.Name);
    }

    [Fact]
    public void WithLoggingEventIds_WithNullConfig_ThrowsArgumentException()
    {
        var builder = SecureHeadersMiddlewareBuilder.CreateBuilder();

        var exception = Record.Exception(() => builder.WithLoggingEventIds(null));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Contains("cannot be null when configuring logging event IDs", exception.Message);
    }

    [Theory]
    [InlineData(5000)]
    [InlineData(10000)]
    [InlineData(0)]
    [InlineData(-1000)]
    public void WithLoggingEventIdBase_SetsOffsetEventIds(int baseEventId)
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIdBase(baseEventId)
            .Build();

        Assert.Equal(baseEventId + 1, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal(baseEventId + 2, config.LoggingConfiguration.HeadersAdded.Id);
        Assert.Equal(baseEventId + 3, config.LoggingConfiguration.RequestIgnored.Id);
        Assert.Equal(baseEventId + 4, config.LoggingConfiguration.HeadersGenerated.Id);
        Assert.Equal(baseEventId + 5, config.LoggingConfiguration.HeaderAdded.Id);
        Assert.Equal(baseEventId + 101, config.LoggingConfiguration.HeaderAdditionFailed.Id);
        Assert.Equal(baseEventId + 102, config.LoggingConfiguration.HeaderRemovalFailed.Id);
        Assert.Equal(baseEventId + 103, config.LoggingConfiguration.ConfigurationIssue.Id);
        Assert.Equal(baseEventId + 201, config.LoggingConfiguration.ConfigurationError.Id);
        Assert.Equal(baseEventId + 202, config.LoggingConfiguration.MiddlewareException.Id);
    }

    [Fact]
    public void WithLoggingEventIdBase_PreservesEventIdNames()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .WithLoggingEventIdBase(5000)
            .Build();

        Assert.Equal("MiddlewareInitialized", config.LoggingConfiguration.MiddlewareInitialized.Name);
        Assert.Equal("HeadersAdded", config.LoggingConfiguration.HeadersAdded.Name);
        Assert.Equal("RequestIgnored", config.LoggingConfiguration.RequestIgnored.Name);
        Assert.Equal("HeadersGenerated", config.LoggingConfiguration.HeadersGenerated.Name);
        Assert.Equal("HeaderAdded", config.LoggingConfiguration.HeaderAdded.Name);
        Assert.Equal("HeaderAdditionFailed", config.LoggingConfiguration.HeaderAdditionFailed.Name);
        Assert.Equal("HeaderRemovalFailed", config.LoggingConfiguration.HeaderRemovalFailed.Name);
        Assert.Equal("ConfigurationIssue", config.LoggingConfiguration.ConfigurationIssue.Name);
        Assert.Equal("ConfigurationError", config.LoggingConfiguration.ConfigurationError.Name);
        Assert.Equal("MiddlewareException", config.LoggingConfiguration.MiddlewareException.Name);
    }

    [Fact]
    public void Builder_WithoutLoggingConfiguration_UsesDefaults()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .Build();

        Assert.NotNull(config.LoggingConfiguration);
        Assert.Equal(1001, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal(1002, config.LoggingConfiguration.HeadersAdded.Id);
        Assert.Equal(2001, config.LoggingConfiguration.HeaderAdditionFailed.Id);
        Assert.Equal(3001, config.LoggingConfiguration.ConfigurationError.Id);
    }

    [Fact]
    public void Builder_MethodChaining_WithLoggingEventIds()
    {
        var customConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(8001, "ChainInit")
        };

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .WithLoggingEventIds(customConfig)
            .UseReferrerPolicy()
            .Build();

        Assert.Equal(8001, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal("ChainInit", config.LoggingConfiguration.MiddlewareInitialized.Name);
        Assert.True(config.UseHsts);
        Assert.True(config.UseXFrameOptions);
        Assert.True(config.UseXContentTypeOptions);
        Assert.True(config.UseReferrerPolicy);
    }

    [Fact]
    public void Builder_MethodChaining_WithLoggingEventIdBase()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIdBase(7000)
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .Build();

        Assert.Equal(7001, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal(7002, config.LoggingConfiguration.HeadersAdded.Id);
        Assert.True(config.UseHsts);
        Assert.True(config.UseXFrameOptions);
        Assert.True(config.UseXContentTypeOptions);
    }

    [Fact]
    public void Builder_LastLoggingConfigurationWins()
    {
        var firstConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(8001, "First")
        };

        var secondConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new EventId(9001, "Second")
        };

        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .WithLoggingEventIds(firstConfig)
            .WithLoggingEventIdBase(7000)
            .WithLoggingEventIds(secondConfig)
            .Build();

        Assert.Equal(9001, config.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal("Second", config.LoggingConfiguration.MiddlewareInitialized.Name);
    }

    [Fact]
    public void LoggingConfiguration_IsIndependentFromSecuritySettings()
    {
        var config1 = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .WithLoggingEventIdBase(5000)
            .Build();

        var config2 = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseXFrameOptions()
            .WithLoggingEventIdBase(6000)
            .Build();

        Assert.Equal(5001, config1.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.Equal(6001, config2.LoggingConfiguration.MiddlewareInitialized.Id);
        Assert.True(config1.UseHsts);
        Assert.False(config1.UseXFrameOptions);
        Assert.False(config2.UseHsts);
        Assert.True(config2.UseXFrameOptions);
    }
}
