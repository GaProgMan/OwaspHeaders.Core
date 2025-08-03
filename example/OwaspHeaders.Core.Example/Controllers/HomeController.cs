using Microsoft.AspNetCore.Mvc;

namespace OwaspHeaders.Core.Example.Controllers;

[ApiController]
[Route("/")]
public class HomeController(ILogger<HomeController> logger) : ControllerBase
{
    private readonly ILogger<HomeController> _logger = logger;

    /// <summary>
    /// Default endpoint that demonstrates SecureHeaders middleware in action.
    /// Check the console logs to see middleware logging output with Event IDs 1001-1005.
    /// </summary>
    [HttpGet(Name = "/")]
    public ActionResult<object> Get()
    {
        _logger.LogInformation("Processing request to default endpoint - SecureHeaders will be applied");

        var response = new
        {
            message = "SecureHeaders middleware applied successfully!",
            headers = GetHeaders,
            loggingInfo = new
            {
                note = "Check console output for SecureHeaders logging with Event IDs 1001-1005",
                eventIds = new
                {
                    middlewareInitialized = 1001,
                    headersAdded = 1002,
                    requestIgnored = 1003,
                    headersGenerated = 1004,
                    headerAdded = 1005
                }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// This endpoint is configured to be ignored by SecureHeaders middleware.
    /// Check the console logs to see the "Request ignored" log with Event ID 1003.
    /// </summary>
    [HttpGet("skipthis", Name = "SkipThis")]
    public ActionResult<object> SkipThis()
    {
        _logger.LogInformation("Processing request to /skipthis - SecureHeaders will be ignored");

        var response = new
        {
            message = "This endpoint bypasses SecureHeaders middleware",
            headers = GetHeaders,
            loggingInfo = new
            {
                note = "Check console output for 'Request ignored' log with Event ID 1003",
                eventId = 1003
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Endpoint to demonstrate the difference between secured and ignored requests
    /// </summary>
    [HttpGet("info", Name = "Info")]
    public ActionResult<object> GetInfo()
    {
        _logger.LogInformation("Providing information about SecureHeaders logging functionality");

        var response = new
        {
            secureHeadersLogging = new
            {
                description = "OwaspHeaders.Core now includes comprehensive logging functionality",
                features = new[]
                {
                    "Information-level logs for successful operations",
                    "Warning logs for configuration issues",
                    "Error logs for validation failures",
                    "Debug logs for detailed header addition information",
                    "Configurable Event IDs to avoid application conflicts",
                    "High-performance logging with log level checking"
                },
                defaultEventIdRanges = new
                {
                    information = "1000-1999",
                    warning = "2000-2999",
                    error = "3000-3999"
                },
                customization = new
                {
                    baseEventId = "Use WithLoggingEventIdBase(baseId) to offset all Event IDs",
                    customEventIds = "Use WithLoggingEventIds(config) for full control over individual Event IDs"
                }
            },
            endpoints = new
            {
                root = "/ - Shows SecureHeaders in action with logging",
                skipthis = "/skipthis - Demonstrates ignored requests with logging",
                info = "/info - This endpoint with SecureHeaders information"
            }
        };

        return Ok(response);
    }

    private IEnumerable<string> GetHeaders => HttpContext.Response.Headers.Select(h => $"{h.Key}: {h.Value}").ToArray();
}
