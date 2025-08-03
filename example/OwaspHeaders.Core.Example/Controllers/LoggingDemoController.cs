using Microsoft.AspNetCore.Mvc;

namespace OwaspHeaders.Core.Example.Controllers;

/// <summary>
/// This controller demonstrates various SecureHeaders logging configurations and scenarios.
/// It shows different ways to configure Event IDs and demonstrates the logging output.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoggingDemoController(ILogger<LoggingDemoController> logger) : ControllerBase
{
    private readonly ILogger<LoggingDemoController> _logger = logger;

    /// <summary>
    /// Shows what the default SecureHeaders configuration logs look like
    /// </summary>
    [HttpGet("default-config")]
    public ActionResult<object> GetDefaultConfig()
    {
        _logger.LogInformation("Demonstrating default SecureHeaders logging configuration");

        var response = new
        {
            description = "This response uses the default SecureHeaders configuration from Program.cs",
            defaultEventIds = new
            {
                middlewareInitialized = 1001,
                headersAdded = 1002,
                requestIgnored = 1003,
                headersGenerated = 1004,
                headerAdded = 1005,
                headerAdditionFailed = 2001,
                headerRemovalFailed = 2002,
                configurationIssue = 2003,
                configurationError = 3001,
                middlewareException = 3002
            },
            logCheckInstructions = "Check your console output for logs with these Event IDs when this endpoint is called"
        };

        return Ok(response);
    }

    /// <summary>
    /// Shows example configuration using custom base Event ID
    /// </summary>
    [HttpGet("custom-base-example")]
    public ActionResult<object> GetCustomBaseExample()
    {
        _logger.LogInformation("Showing example of custom base Event ID configuration");

        var customConfigExample = @"
// Example: Using custom base Event ID (offset all Event IDs by 5000)
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .WithLoggingEventIdBase(5000)  // Event IDs will be 5001, 5002, 5003, etc.
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
";

        var response = new
        {
            description = "Example of configuring SecureHeaders with custom base Event ID",
            codeExample = customConfigExample,
            resultingEventIds = new
            {
                middlewareInitialized = 5001,
                headersAdded = 5002,
                requestIgnored = 5003,
                headersGenerated = 5004,
                headerAdded = 5005,
                note = "All Event IDs are offset by the base value (5000)"
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Shows example configuration using fully custom Event IDs
    /// </summary>
    [HttpGet("fully-custom-example")]
    public ActionResult<object> GetFullyCustomExample()
    {
        _logger.LogInformation("Showing example of fully custom Event ID configuration");

        var fullyCustomConfigExample = @"
// Example: Fully custom Event ID configuration
var customLoggingConfig = new SecureHeadersLoggingConfiguration
{
    MiddlewareInitialized = new EventId(9001, ""SecureHeadersInit""),
    HeadersAdded = new EventId(9002, ""HeadersSet""),
    RequestIgnored = new EventId(9003, ""RequestSkipped""),
    HeadersGenerated = new EventId(9004, ""HeadersBuilt""),
    HeaderAdded = new EventId(9005, ""SingleHeaderAdded""),
    ConfigurationError = new EventId(9999, ""ConfigError"")
};

var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .WithLoggingEventIds(customLoggingConfig)
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
";

        var response = new
        {
            description = "Example of configuring SecureHeaders with fully custom Event IDs",
            codeExample = fullyCustomConfigExample,
            benefits = new[]
            {
                "Complete control over Event ID values",
                "Custom Event ID names for better identification",
                "Can integrate with existing application Event ID schemes",
                "Avoid conflicts with other middleware Event IDs"
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Demonstrates what happens when you have multiple SecureHeaders configurations
    /// </summary>
    [HttpGet("multiple-configs")]
    public ActionResult<object> GetMultipleConfigsInfo()
    {
        _logger.LogInformation("Explaining multiple SecureHeaders configurations");

        var response = new
        {
            warning = "Only one SecureHeaders middleware should be registered per application",
            currentImplementation = "This example uses the default configuration from Program.cs",
            alternativeApproaches = new
            {
                environmentBasedConfig = "Use different configurations based on Development/Production environments",
                featureFlagConfig = "Enable/disable specific headers based on feature flags",
                dynamicConfig = "Load configuration from appsettings.json or external sources"
            },
            bestPractice = "Choose one configuration approach and stick with it throughout your application"
        };

        return Ok(response);
    }

    /// <summary>
    /// Shows how to troubleshoot SecureHeaders logging issues
    /// </summary>
    [HttpGet("troubleshooting")]
    public ActionResult<object> GetTroubleshootingInfo()
    {
        _logger.LogInformation("Providing SecureHeaders logging troubleshooting information");

        var response = new
        {
            commonIssues = new
            {
                noLogsVisible = new
                {
                    problem = "SecureHeaders logs are not appearing in console",
                    solutions = new[]
                    {
                        "Ensure logging level is set to Debug or Information",
                        "Check that console logging provider is added",
                        "Verify SecureHeaders middleware is registered with ILogger parameter"
                    }
                },
                eventIdConflicts = new
                {
                    problem = "Event ID conflicts with other middleware",
                    solutions = new[]
                    {
                        "Use WithLoggingEventIdBase() to offset Event IDs",
                        "Use WithLoggingEventIds() for complete control",
                        "Check your application's Event ID usage patterns"
                    }
                },
                performanceConcerns = new
                {
                    problem = "Worried about logging performance impact",
                    explanation = "SecureHeaders uses high-performance logging patterns with log level checking",
                    details = "Logging calls are only made when the log level is enabled, minimizing performance impact"
                }
            },
            debuggingTips = new[]
            {
                "Set logging level to Debug to see individual header additions",
                "Use structured logging to filter SecureHeaders events",
                "Check Event IDs to identify specific SecureHeaders log entries",
                "Monitor both Information and Warning level logs for complete picture"
            }
        };

        return Ok(response);
    }
}
