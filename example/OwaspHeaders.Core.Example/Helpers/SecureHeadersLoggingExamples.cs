using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Example.Helpers;

/// <summary>
/// This class provides various examples of how to configure SecureHeaders with different logging options.
/// Each is a configure delegate, so it can be handed straight to app.UseSecureHeadersMiddleware,
/// and asserted on from a test via SecureHeadersBuilder.BuildAndValidate.
/// </summary>
public static class SecureHeadersLoggingExamples
{
    /// <summary>
    /// Basic configuration with default Event IDs (1000-3999 range)
    /// Suitable for most applications that don't have Event ID conflicts
    /// </summary>
    public static void ConfigureBasic(SecureHeadersBuilder opt)
    {
        opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseReferrerPolicy();
    }

    /// <summary>
    /// Configuration with custom base Event ID to avoid conflicts
    /// Use this when your application already uses Event IDs in the 1000-3999 range
    /// </summary>
    public static void ConfigureCustomBase(SecureHeadersBuilder opt, int baseEventId = 5000)
    {
        opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseReferrerPolicy()
            .UseCrossOriginResourcePolicy()
            .WithLoggingEventIdBase(baseEventId);
    }

    /// <summary>
    /// Configuration with fully custom Event IDs for complete control
    /// Use this when you need specific Event ID values and names
    /// </summary>
    public static void ConfigureFullyCustom(SecureHeadersBuilder opt)
    {
        var customLoggingConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new Microsoft.Extensions.Logging.EventId(8001, "SecureHeadersStarted"),
            HeadersAdded = new Microsoft.Extensions.Logging.EventId(8002, "SecurityHeadersApplied"),
            RequestIgnored = new Microsoft.Extensions.Logging.EventId(8003, "RequestSkippedByRule"),
            HeadersGenerated = new Microsoft.Extensions.Logging.EventId(8004, "SecurityHeadersGenerated"),
            HeaderAdded = new Microsoft.Extensions.Logging.EventId(8005, "IndividualHeaderAdded"),
            HeaderAdditionFailed = new Microsoft.Extensions.Logging.EventId(8101, "HeaderAddFailed"),
            HeaderRemovalFailed = new Microsoft.Extensions.Logging.EventId(8102, "HeaderRemoveFailed"),
            ConfigurationIssue = new Microsoft.Extensions.Logging.EventId(8201, "ConfigurationWarning"),
            ConfigurationError = new Microsoft.Extensions.Logging.EventId(8301, "ConfigurationFailed"),
            MiddlewareException = new Microsoft.Extensions.Logging.EventId(8302, "MiddlewareError")
        };

        opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseReferrerPolicy()
            .UseCrossOriginResourcePolicy()
            .WithLoggingEventIds(customLoggingConfig);
    }

    /// <summary>
    /// Comprehensive security configuration with logging for production use
    /// Includes most OWASP recommended headers with custom Event IDs
    /// </summary>
    public static void ConfigureProduction(SecureHeadersBuilder opt)
    {
        opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseReferrerPolicy()
            .UseCrossOriginResourcePolicy()
            .UseCrossOriginOpenerPolicy()
            .UseContentSecurityPolicy()
            .UseCacheControl()
            .WithLoggingEventIdBase(6000); // Use 6000 range for production
    }

    /// <summary>
    /// Development-friendly configuration with verbose logging
    /// Suitable for development environments where you want to see all activity
    /// </summary>
    public static void ConfigureDevelopment(SecureHeadersBuilder opt)
    {
        var devLoggingConfig = new SecureHeadersLoggingConfiguration
        {
            MiddlewareInitialized = new Microsoft.Extensions.Logging.EventId(9001, "DEV_SecureHeadersInit"),
            HeadersAdded = new Microsoft.Extensions.Logging.EventId(9002, "DEV_HeadersApplied"),
            RequestIgnored = new Microsoft.Extensions.Logging.EventId(9003, "DEV_RequestIgnored"),
            HeadersGenerated = new Microsoft.Extensions.Logging.EventId(9004, "DEV_HeadersGenerated"),
            HeaderAdded = new Microsoft.Extensions.Logging.EventId(9005, "DEV_HeaderAdded"),
            HeaderAdditionFailed = new Microsoft.Extensions.Logging.EventId(9101, "DEV_HeaderAddFailed"),
            HeaderRemovalFailed = new Microsoft.Extensions.Logging.EventId(9102, "DEV_HeaderRemoveFailed"),
            ConfigurationIssue = new Microsoft.Extensions.Logging.EventId(9201, "DEV_ConfigIssue"),
            ConfigurationError = new Microsoft.Extensions.Logging.EventId(9301, "DEV_ConfigError"),
            MiddlewareException = new Microsoft.Extensions.Logging.EventId(9302, "DEV_MiddlewareException")
        };

        opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseReferrerPolicy()
            .WithLoggingEventIds(devLoggingConfig);
    }

    /// <summary>
    /// Configuration that demonstrates Event ID conflict resolution
    /// Shows how to work around existing application Event IDs
    /// </summary>
    public static SecureHeadersMiddlewareConfiguration GetConflictFreeConfiguration(int[] existingEventIds)
    {
        // Find a safe range that doesn't conflict with existing Event IDs
        int baseEventId = 10000;
        while (existingEventIds.Any(id => id >= baseEventId && id < baseEventId + 1000))
        {
            baseEventId += 1000;
        }

        return new SecureHeadersBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .WithLoggingEventIdBase(baseEventId)
            .Build();
    }

    /// <summary>
    /// Minimal configuration with logging disabled
    /// Use this when you want SecureHeaders functionality but no logging overhead
    /// Note: This still allows logging but uses null logger, so no performance impact
    /// </summary>
    public static void ConfigureMinimal(SecureHeadersBuilder opt)
    {
        // Note: When no logger is provided to middleware constructor, logging is automatically disabled
        opt
            .UseHsts()
            .UseXFrameOptions();
    }
}

/// <summary>
/// Extension methods to make it easier to use the logging examples
/// </summary>
public static class LoggingExampleExtensions
{
    /// <summary>
    /// Use SecureHeaders with basic logging configuration
    /// </summary>
    public static IApplicationBuilder UseSecureHeadersWithBasicLogging(this IApplicationBuilder app)
    {
        return app.UseSecureHeadersMiddleware(SecureHeadersLoggingExamples.ConfigureBasic);
    }

    /// <summary>
    /// Use SecureHeaders with custom base Event ID
    /// </summary>
    public static IApplicationBuilder UseSecureHeadersWithCustomEventIds(this IApplicationBuilder app, int baseEventId)
    {
        return app.UseSecureHeadersMiddleware(opt => SecureHeadersLoggingExamples.ConfigureCustomBase(opt, baseEventId));
    }

    /// <summary>
    /// Use SecureHeaders with production-ready configuration
    /// </summary>
    public static IApplicationBuilder UseSecureHeadersForProduction(this IApplicationBuilder app)
    {
        return app.UseSecureHeadersMiddleware(SecureHeadersLoggingExamples.ConfigureProduction);
    }

    /// <summary>
    /// Use SecureHeaders with development-friendly configuration
    /// </summary>
    public static IApplicationBuilder UseSecureHeadersForDevelopment(this IApplicationBuilder app)
    {
        return app.UseSecureHeadersMiddleware(SecureHeadersLoggingExamples.ConfigureDevelopment);
    }
}
