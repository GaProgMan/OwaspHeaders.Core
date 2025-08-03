# OwaspHeaders.Core Example Application

This example application demonstrates how to use the OwaspHeaders.Core middleware with the new logging functionality.

## Getting Started

1. Run the application:

```bash
dotnet run
```

2. Open your browser to the displayed URL (typically `https://localhost:7xxx`, check the `launchSettings.json` file for the exact port)

3. Navigate to the Swagger UI to explore the available endpoints

## Available Endpoints

### Basic Endpoints

- **GET /**: Default endpoint with SecureHeaders applied - demonstrates middleware logging
- **GET /skipthis**: Endpoint that bypasses SecureHeaders - demonstrates "request ignored" logging  
- **GET /info**: Information about SecureHeaders logging functionality

### Logging Demo Endpoints

- **GET /api/LoggingDemo/default-config**: Shows default logging configuration
- **GET /api/LoggingDemo/custom-base-example**: Example of custom base Event ID configuration
- **GET /api/LoggingDemo/fully-custom-example**: Example of fully custom Event ID configuration
- **GET /api/LoggingDemo/multiple-configs**: Information about multiple configurations
- **GET /api/LoggingDemo/troubleshooting**: Troubleshooting guide for logging issues

## Observing SecureHeaders Logging

### Console Output

When you run the application, you'll see SecureHeaders logs in the console with Event IDs:

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

### Log Levels and Event IDs

| Log Level | Event ID Range | Description |
|-----------|----------------|-------------|
| Information | 1001-1999 | Normal operations (initialization, headers added) |
| Warning | 2001-2999 | Configuration issues, header add/remove failures |
| Error | 3001-3999 | Configuration errors, middleware exceptions |
| Debug | 1005 | Individual header addition details |

### Specific Event IDs

- **1001**: Middleware initialized
- **1002**: Headers added to response
- **1003**: Request ignored due to URL exclusion rule
- **1004**: Security headers generated
- **1005**: Individual header added (Debug level)
- **2001**: Header addition failed
- **2002**: Header removal failed
- **2003**: Configuration issue detected
- **3001**: Configuration validation failed
- **3002**: Middleware exception occurred

## Logging Configuration Examples

### 1. Basic Configuration (Default Event IDs)

```csharp
// Uses Event IDs 1001-3999
app.UseSecureHeadersMiddleware();
```

### 2. Custom Base Event ID

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .WithLoggingEventIdBase(5000)  // Event IDs will be 5001, 5002, etc.
    .Build();

app.UseSecureHeadersMiddleware(config);
```

### 3. Fully Custom Event IDs

```csharp
var customLogging = new SecureHeadersLoggingConfiguration
{
    MiddlewareInitialized = new EventId(9001, "SecureHeadersInit"),
    HeadersAdded = new EventId(9002, "HeadersSet"),
    ConfigurationError = new EventId(9999, "ConfigError")
};

var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .WithLoggingEventIds(customLogging)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

### 4. Using Helper Extension Methods

```csharp
// From SecureHeadersLoggingExamples helper class
app.UseSecureHeadersWithBasicLogging();
app.UseSecureHeadersWithCustomEventIds(5000);
app.UseSecureHeadersForProduction();
app.UseSecureHeadersForDevelopment();
```

## Configuring Log Levels

### In appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "OwaspHeaders.Core": "Debug"  // See all SecureHeaders logs
    }
  }
}
```

### Programmatically

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Files in This Example

- **Program.cs**: Main application setup with logging configuration examples
- **Controllers/HomeController.cs**: Basic endpoints demonstrating SecureHeaders
- **Controllers/LoggingDemoController.cs**: Dedicated endpoints for logging demonstrations
- **Helpers/SecureHeadersLoggingExamples.cs**: Helper class with reusable configuration examples
- **appsettings.json**: Production logging configuration
- **appsettings.Development.json**: Development logging configuration with debug output

## Performance Notes

SecureHeaders logging uses high-performance patterns:
- Log level checking before constructing log messages
- Structured logging with named parameters
- Minimal string allocation
- No performance impact when logging is disabled

## Troubleshooting

1. **Not seeing logs**: Check that logging level includes Information or Debug for `OwaspHeaders.Core`
2. **Event ID conflicts**: Use `WithLoggingEventIdBase()` or `WithLoggingEventIds()` to customize
3. **Too much output**: Set `OwaspHeaders.Core` log level to Warning or Error to reduce verbosity
4. **No middleware activity**: Ensure middleware is registered correctly and requests aren't being ignored

For more examples and configurations, explore the `/api/LoggingDemo` endpoints in the running application.
