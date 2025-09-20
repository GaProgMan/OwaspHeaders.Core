# Clear-Site-Data Middleware Integration API Contract

## SecureHeadersMiddleware Extension

### GenerateRelevantHeaders Method Modification

#### Integration Point
Adds Clear-Site-Data header generation to existing `GenerateRelevantHeaders()` method in `SecureHeadersMiddleware.cs`.

#### Implementation Contract
```csharp
// Add to existing GenerateRelevantHeaders() method
private FrozenDictionary<string, string> GenerateRelevantHeaders()
{
    var temporaryDictionary = new Dictionary<string, string>();

    // ... existing headers ...

    if (_config.UseClearSiteData)
    {
        var clearSiteDataHeaders = GenerateClearSiteDataHeaders();
        foreach (var (path, headerValue) in clearSiteDataHeaders)
        {
            // Path-specific headers handled during request processing
            // Global headers added here if defaultConfiguration exists
        }
    }

    return temporaryDictionary.ToFrozenDictionary();
}
```

### Request Processing Modification

#### InvokeAsync Method Enhancement
Adds path-specific Clear-Site-Data header processing to existing request handling.

#### Implementation Contract
```csharp
public async Task InvokeAsync(HttpContext httpContext)
{
    // ... existing validation and initialization ...

    if (!RequestShouldBeIgnored(httpContext.Request.Path))
    {
        // ... existing header processing ...

        // Add Clear-Site-Data path-specific processing
        if (_config.UseClearSiteData && _config.ClearSiteDataPathConfiguration != null)
        {
            var clearSiteDataHeader = GenerateClearSiteDataForPath(httpContext.Request.Path);
            if (!string.IsNullOrEmpty(clearSiteDataHeader))
            {
                var headerFailedEventId = _config?.LoggingConfiguration?.HeaderAdditionFailed ?? SecureHeadersEventIds.HeaderAdditionFailed;
                if (httpContext.TryAddHeader(Constants.ClearSiteDataHeaderName, clearSiteDataHeader, _logger, headerFailedEventId))
                {
                    LogHeaderAdded(Constants.ClearSiteDataHeaderName, clearSiteDataHeader.Length);
                }
            }
        }
    }

    await _next(httpContext);
}
```

## Path Resolution API

### GenerateClearSiteDataForPath Method

#### Method Signature
```csharp
private string GenerateClearSiteDataForPath(PathString requestPath)
```

#### Purpose
Resolves path-specific Clear-Site-Data configuration and generates appropriate header value.

#### Parameters
- `requestPath`: The current request path from HttpContext

#### Return Value
- Returns header value string if path matches configuration
- Returns null or empty string if no configuration matches

#### Behavior
- Performs exact path matching using longest-match precedence
- Calls `BuildHeaderValue()` on resolved configuration
- Handles null configurations gracefully
- Thread-safe read-only operations

#### Performance Contract
- O(1) average case for path lookup
- O(n) worst case for precedence resolution
- No memory allocations during request processing

### Path Matching Logic
```csharp
private ClearSiteDataConfiguration GetConfigurationForPath(PathString requestPath)
{
    if (!requestPath.HasValue || _config.ClearSiteDataPathConfiguration == null)
        return null;

    return _config.ClearSiteDataPathConfiguration.GetConfigurationForPath(requestPath.Value);
}
```

## Configuration Integration Contract

### SecureHeadersMiddlewareConfiguration Extensions

#### New Properties
```csharp
public class SecureHeadersMiddlewareConfiguration
{
    // Existing properties...

    /// <summary>
    /// Indicates whether the response should use Clear-Site-Data header
    /// </summary>
    public bool UseClearSiteData { get; set; }

    /// <summary>
    /// The Clear-Site-Data path-specific configuration to use
    /// </summary>
    public ClearSiteDataPathConfiguration ClearSiteDataPathConfiguration { get; set; }
}
```

#### Validation Rules
- `UseClearSiteData` defaults to false
- `ClearSiteDataPathConfiguration` can be null (no path-specific behavior)
- Configuration immutable after Build() call

## Header Generation Contract

### Constants Integration
```csharp
public static class Constants
{
    // Existing constants...
    public const string ClearSiteDataHeaderName = "Clear-Site-Data";
}
```

### Header Value Format
- Global headers: Standard quoted-string format per W3C spec
- Path-specific headers: Same format, resolved per request
- Wildcard precedence: `"*"` overrides all other directives
- Multiple directives: Comma-separated quoted strings

#### Examples
```
Clear-Site-Data: "*"
Clear-Site-Data: "cache","cookies","storage"
Clear-Site-Data: "cookies"
```

## Logging Integration Contract

### Existing Log Methods Extension

#### LogHeaderAdded Usage
```csharp
LogHeaderAdded(Constants.ClearSiteDataHeaderName, headerValue.Length);
```

#### Error Logging
Uses existing logging patterns:
- Configuration errors: `LogConfigurationError()`
- Header addition failures: Existing `HeaderAdditionFailed` event ID
- Debug information: Existing `HeaderAdded` event ID

### New Log Events (Optional)
```csharp
// If needed for Clear-Site-Data specific logging
private void LogClearSiteDataPathResolved(string requestPath, string configuration)
{
    if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
    {
        _logger.Log(LogLevel.Debug,
            "Clear-Site-Data configuration resolved for path {RequestPath}: {Configuration}",
            requestPath, configuration);
    }
}
```

## Thread Safety Contract

### Read-Only Operations
- All request processing uses read-only configuration access
- Path resolution performs no modifications
- Header generation is stateless

### Configuration Immutability
- `ClearSiteDataPathConfiguration` frozen after Build()
- Path dictionary immutable after construction
- Configuration objects thread-safe for concurrent requests

## Error Handling Contract

### Configuration Validation
- Invalid configurations caught during Build() phase
- Runtime errors logged but don't block request processing
- Graceful degradation when Clear-Site-Data config is null

### Request Processing Errors
- Header addition failures logged using existing patterns
- Path resolution failures return null (no header added)
- Exception handling follows existing middleware patterns

## Performance Contract

### Memory Usage
- No additional allocations per request for path lookup
- Pre-computed header values where possible
- Minimal overhead for non-configured paths

### Processing Time
- Target: <1ms additional processing per request
- Early exit for non-configured paths
- Optimized path matching with sorted precedence

## Backward Compatibility Contract

### Existing Behavior Preservation
- No changes to existing header processing
- No impact on non-Clear-Site-Data requests
- Existing configuration methods unchanged

### Default State
- Clear-Site-Data disabled by default
- Zero performance impact when not configured
- No breaking changes to existing API surface