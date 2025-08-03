---
title: Logging
layout: page
nav_order: 4
---

# Logging

OwaspHeaders.Core includes comprehensive logging functionality to help developers monitor middleware operations, troubleshoot configuration issues, and audit security header application.

## Overview

The logging system provides:
- **Information-level logs** for successful operations
- **Warning logs** for configuration issues
- **Error logs** for validation failures
- **Debug logs** for detailed header information
- **Configurable Event IDs** to avoid application conflicts
- **High-performance patterns** with minimal overhead

## Event ID Schema

| Log Level | Event ID Range | Description |
|-----------|----------------|-------------|
| Information | 1001-1999 | Normal operations (initialization, headers added) |
| Warning | 2001-2999 | Configuration issues, header failures |
| Error | 3001-3999 | Configuration errors, exceptions |
| Debug | 1005 | Individual header addition details |

### Specific Event IDs

- **1001**: Middleware initialized with N headers enabled
- **1002**: Added N security headers to response for {path}
- **1003**: Request ignored due to URL exclusion rule
- **1004**: Generated N security headers
- **1005**: Added header {name} with value length {length}
- **2001**: Header addition failed for {header}
- **2002**: Header removal failed for {header}
- **2003**: Configuration issue detected
- **3001**: Configuration validation failed
- **3002**: Middleware exception occurred

## Basic Configuration

### Default Logging (Recommended)

```csharp
// in Program.cs
app.UseSecureHeadersMiddleware();
```

Logging is automatically enabled when an `ILogger<SecureHeadersMiddleware>` is available in dependency injection.

### Console Output Example

```
info: OwaspHeaders.Core.SecureHeadersMiddleware[1001]
      SecureHeaders middleware initialized with 4 headers enabled
info: OwaspHeaders.Core.SecureHeadersMiddleware[1004]
      Generated 4 security headers
info: OwaspHeaders.Core.SecureHeadersMiddleware[1002]
      Added 4 security headers to response for /
dbug: OwaspHeaders.Core.SecureHeadersMiddleware[1005]
      Added header Strict-Transport-Security with value length 37
```

## Custom Event ID Configuration

### Option 1: Custom Base Event ID

Use this to offset all Event IDs when your application uses the default 1000-3999 range:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .WithLoggingEventIdBase(5000)  // Event IDs become 5001, 5002, etc.
    .Build();

app.UseSecureHeadersMiddleware(config);
```

### Option 2: Fully Custom Event IDs

For complete control over individual Event IDs:

```csharp
var customLogging = new SecureHeadersLoggingConfiguration
{
    MiddlewareInitialized = new EventId(9001, "SecureHeadersInit"),
    HeadersAdded = new EventId(9002, "HeadersSet"),
    RequestIgnored = new EventId(9003, "RequestSkipped"),
    ConfigurationError = new EventId(9999, "ConfigError")
};

var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .WithLoggingEventIds(customLogging)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

## Log Level Configuration

### In appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "OwaspHeaders.Core": "Debug"
    }
  }
}
```

### Programmatically

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Performance Considerations

The logging implementation uses high-performance patterns:
- **Log level checking**: Expensive operations only occur when logging is enabled
- **Structured logging**: Named parameters for efficient log processing
- **Minimal allocation**: Reduced string creation and memory pressure
- **Zero overhead**: No performance impact when logger is null or disabled

## Troubleshooting

### Common Issues

**Not seeing logs**
- Ensure logging level includes Information or Debug for `OwaspHeaders.Core`
- Verify console logging provider is registered
- Check that SecureHeaders middleware is properly configured

**Event ID conflicts**
- Use `WithLoggingEventIdBase()` to offset all Event IDs
- Use `WithLoggingEventIds()` for complete control
- Review your application's Event ID usage patterns

**Too much log output**
- Set `OwaspHeaders.Core` log level to Warning or Error
- Use structured logging filters to target specific Event IDs
- Consider separate log sinks for security audit logs

### Debug Tips

1. Set logging to Debug level to see individual header operations
2. Use Event IDs to filter SecureHeaders-specific log entries
3. Monitor both Information and Warning levels for complete visibility
4. Check for "Request ignored" logs (Event ID 1003) for URL exclusions

## Backward Compatibility

The logging functionality is **100% backward compatible**:
- Existing applications work without changes
- Optional `ILogger` parameter defaults to null (no logging)
- No performance impact on existing installations
- All existing configurations continue to work

## Example Application

See the [example application](https://github.com/GaProgMan/OwaspHeaders.Core/tree/main/example/OwaspHeaders.Core.Example) for comprehensive logging demonstrations including:
- Different configuration approaches
- Interactive API endpoints showing logging output
- Helper classes with reusable configurations
- Complete documentation and troubleshooting examples
