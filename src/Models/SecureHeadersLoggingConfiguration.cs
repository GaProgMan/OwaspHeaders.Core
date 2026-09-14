namespace OwaspHeaders.Core.Models;

/// <summary>
/// Configuration for SecureHeaders logging Event IDs.
/// Allows customization to avoid conflicts with application event IDs.
/// </summary>
public class SecureHeadersLoggingConfiguration
{
    /// <summary>
    /// Event ID for middleware initialization logging
    /// </summary>
    public EventId MiddlewareInitialized { get; set; } = SecureHeadersEventIds.MiddlewareInitialized;

    /// <summary>
    /// Event ID for successful header addition logging
    /// </summary>
    public EventId HeadersAdded { get; set; } = SecureHeadersEventIds.HeadersAdded;

    /// <summary>
    /// Event ID for request ignored due to URL exclusion rules
    /// </summary>
    public EventId RequestIgnored { get; set; } = SecureHeadersEventIds.RequestIgnored;

    /// <summary>
    /// Event ID for header generation logging
    /// </summary>
    public EventId HeadersGenerated { get; set; } = SecureHeadersEventIds.HeadersGenerated;

    /// <summary>
    /// Event ID for individual header addition logging
    /// </summary>
    public EventId HeaderAdded { get; set; } = SecureHeadersEventIds.HeaderAdded;

    /// <summary>
    /// Event ID for header addition failure warnings
    /// </summary>
    public EventId HeaderAdditionFailed { get; set; } = SecureHeadersEventIds.HeaderAdditionFailed;

    /// <summary>
    /// Event ID for header removal failure warnings
    /// </summary>
    public EventId HeaderRemovalFailed { get; set; } = SecureHeadersEventIds.HeaderRemovalFailed;

    /// <summary>
    /// Event ID for configuration issue warnings
    /// </summary>
    public EventId ConfigurationIssue { get; set; } = SecureHeadersEventIds.ConfigurationIssue;

    /// <summary>
    /// Event ID for configuration error logging
    /// </summary>
    public EventId ConfigurationError { get; set; } = SecureHeadersEventIds.ConfigurationError;

    /// <summary>
    /// Event ID for middleware exception error logging
    /// </summary>
    public EventId MiddlewareException { get; set; } = SecureHeadersEventIds.MiddlewareException;

    /// <summary>
    /// Factory method to create a logging configuration with event IDs based on a base offset.
    /// Useful for avoiding event ID conflicts with existing application logging.
    /// </summary>
    /// <param name="baseEventId">Base event ID to offset from (e.g., 5000 will create event IDs 5001, 5002, etc.)</param>
    /// <returns>A new SecureHeadersLoggingConfiguration with offset event IDs</returns>
    public static SecureHeadersLoggingConfiguration CreateWithBaseEventId(int baseEventId)
    {
        return new SecureHeadersLoggingConfiguration
        {
            // Information events (baseEventId + 1-99)
            MiddlewareInitialized = new EventId(baseEventId + 1, nameof(MiddlewareInitialized)),
            HeadersAdded = new EventId(baseEventId + 2, nameof(HeadersAdded)),
            RequestIgnored = new EventId(baseEventId + 3, nameof(RequestIgnored)),
            HeadersGenerated = new EventId(baseEventId + 4, nameof(HeadersGenerated)),
            HeaderAdded = new EventId(baseEventId + 5, nameof(HeaderAdded)),

            // Warning events (baseEventId + 100-199)
            HeaderAdditionFailed = new EventId(baseEventId + 101, nameof(HeaderAdditionFailed)),
            HeaderRemovalFailed = new EventId(baseEventId + 102, nameof(HeaderRemovalFailed)),
            ConfigurationIssue = new EventId(baseEventId + 103, nameof(ConfigurationIssue)),

            // Error events (baseEventId + 200-299)
            ConfigurationError = new EventId(baseEventId + 201, nameof(ConfigurationError)),
            MiddlewareException = new EventId(baseEventId + 202, nameof(MiddlewareException))
        };
    }
}
