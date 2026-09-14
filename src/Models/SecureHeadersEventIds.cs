namespace OwaspHeaders.Core.Models;

/// <summary>
/// Defines default Event IDs for SecureHeaders logging events.
/// These can be overridden via SecureHeadersLoggingConfiguration if needed to avoid conflicts.
/// </summary>
public static class SecureHeadersEventIds
{
    // Information Events (1000-1999)

    /// <summary>
    /// Logged when the SecureHeaders middleware is initialized
    /// </summary>
    public static readonly EventId MiddlewareInitialized = new(1001, nameof(MiddlewareInitialized));

    /// <summary>
    /// Logged when security headers are successfully added to a response
    /// </summary>
    public static readonly EventId HeadersAdded = new(1002, nameof(HeadersAdded));

    /// <summary>
    /// Logged when a request is ignored due to URL exclusion rules
    /// </summary>
    public static readonly EventId RequestIgnored = new(1003, nameof(RequestIgnored));

    /// <summary>
    /// Logged when headers are generated for the first time
    /// </summary>
    public static readonly EventId HeadersGenerated = new(1004, nameof(HeadersGenerated));

    /// <summary>
    /// Logged when an individual header is successfully added
    /// </summary>
    public static readonly EventId HeaderAdded = new(1005, nameof(HeaderAdded));

    // Warning Events (2000-2999)

    /// <summary>
    /// Logged when a header fails to be added to the response
    /// </summary>
    public static readonly EventId HeaderAdditionFailed = new(2001, nameof(HeaderAdditionFailed));

    /// <summary>
    /// Logged when a header fails to be removed from the response
    /// </summary>
    public static readonly EventId HeaderRemovalFailed = new(2002, nameof(HeaderRemovalFailed));

    /// <summary>
    /// Logged when a configuration issue is detected
    /// </summary>
    public static readonly EventId ConfigurationIssue = new(2003, nameof(ConfigurationIssue));

    // Error Events (3000-3999)

    /// <summary>
    /// Logged when configuration validation fails
    /// </summary>
    public static readonly EventId ConfigurationError = new(3001, nameof(ConfigurationError));

    /// <summary>
    /// Logged when an unexpected exception occurs in the middleware
    /// </summary>
    public static readonly EventId MiddlewareException = new(3002, nameof(MiddlewareException));
}
