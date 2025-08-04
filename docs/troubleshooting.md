---
title: Troubleshooting
layout: page
nav_order: 8
---

# Troubleshooting

This page covers common issues you might encounter when using OwaspHeaders.Core and their solutions.

## Common Issues

### Headers Not Applied

**Problem**: Security headers are not appearing in HTTP responses.

**Solutions**:
1. **Check middleware registration order** - SecureHeaders should be registered early in the pipeline:

   ```csharp
   // Correct order
   app.UseSecureHeadersMiddleware();
   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllers();
   ```

2. **Verify URL exclusions** - Check if requests are being ignored due to URL exclusion rules:

   ```csharp
   // Check your configuration for ignored URLs
   var config = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseHsts()
       .SetUrlsToIgnore(["/health", "/api/status"])  // These URLs will be ignored
       .Build();
   ```
   Look for Event ID 1003 in logs: "Request ignored due to URL exclusion rule"

3. **Review configuration** - Ensure headers are enabled in your configuration:

   ```csharp
   var config = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseHsts()        // Explicitly enable each header you want
       .UseXFrameOptions()
       .Build();
   ```

### Configuration Problems

**Problem**: Middleware throws configuration errors or warnings.

**Solutions**:

1. **Check logs for specific errors**:
   - Event ID 2003: Configuration issues (warnings)
   - Event ID 3001: Configuration validation failures (errors)

2. **Common configuration issues**:

   ```csharp
   // Problem: COEP without CORP
   var badConfig = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseCrossOriginEmbedderPolicy()  // This requires CORP to be enabled
       .Build();

   // Solution: Enable both headers
   var goodConfig = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseCrossOriginResourcePolicy()
       .UseCrossOriginEmbedderPolicy()
       .Build();
   ```

### Performance Concerns

**Problem**: Worried about logging performance impact.

**Solutions**:
- SecureHeaders logging uses high-performance patterns with minimal overhead
- Log level checking ensures expensive operations only occur when needed
- Set logging level to Warning or Error to reduce output volume
- Logging has zero performance impact when logger is null or disabled

### Logging Issues

**Problem**: Not seeing SecureHeaders logs in output.

**Solutions**:

1. **Ensure logging is configured**:

   ```csharp
   // In Program.cs
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Information);
   ```

2. **Check logging configuration**:

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

3. **Verify middleware registration**:

   ```csharp
   // Logging is automatic when ILogger is available
   app.UseSecureHeadersMiddleware();
   ```

**Problem**: Too much log output.

**Solutions**:

- Set `OwaspHeaders.Core` log level to Warning or Error
- Use structured logging filters to target specific Event IDs
- Consider separate log sinks for security audit logs

**Problem**: Event ID conflicts with application logging.

**Solutions**:

1. **Use custom base Event ID**:

   ```csharp
   var config = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseHsts()
       .WithLoggingEventIdBase(5000)  // Offset all Event IDs
       .Build();
   ```

2. **Use fully custom Event IDs**:

   ```csharp
   var customLogging = new SecureHeadersLoggingConfiguration
   {
       MiddlewareInitialized = new EventId(9001, "SecureHeadersInit"),
       HeadersAdded = new EventId(9002, "HeadersSet")
   };
   
   var config = SecureHeadersMiddlewareBuilder
       .CreateBuilder()
       .UseHsts()
       .WithLoggingEventIds(customLogging)
       .Build();
   ```

## Debug Information

### Enable Debug Logging

To see detailed middleware operations, enable debug logging:

```json
{
  "Logging": {
    "LogLevel": {
      "OwaspHeaders.Core": "Debug"
    }
  }
}
```

This will show individual header additions with Event ID 1005:
```
dbug: OwaspHeaders.Core.SecureHeadersMiddleware[1005]
      Added header Strict-Transport-Security with value length 37
```

### Understanding Log Output

**Information Level (Event IDs 1001-1999)**:
- 1001: Middleware initialization
- 1002: Headers added to response
- 1003: Request ignored (URL exclusion)
- 1004: Headers generated

**Warning Level (Event IDs 2001-2999)**:
- 2001: Header addition failed
- 2002: Header removal failed
- 2003: Configuration issue detected

**Error Level (Event IDs 3001-3999)**:
- 3001: Configuration validation failed
- 3002: Middleware exception occurred

### Common Log Patterns

**Normal Operation**:

```
info: OwaspHeaders.Core.SecureHeadersMiddleware[1001]
      SecureHeaders middleware initialized with 4 headers enabled
info: OwaspHeaders.Core.SecureHeadersMiddleware[1004]
      Generated 4 security headers
info: OwaspHeaders.Core.SecureHeadersMiddleware[1002]
      Added 4 security headers to response for /api/users
```

**URL Exclusion**:

```
info: OwaspHeaders.Core.SecureHeadersMiddleware[1003]
      Request ignored due to URL exclusion rule: /health
```

**Configuration Problem**:

```
warn: OwaspHeaders.Core.SecureHeadersMiddleware[2003]
      Configuration issue detected: Cross-Origin-Embedder-Policy requires Cross-Origin-Resource-Policy to be enabled
```

## Getting Help

If you're still experiencing issues:

1. **Check the [example application](https://github.com/GaProgMan/OwaspHeaders.Core/tree/main/example/OwaspHeaders.Core.Example)** for working configurations
2. **Review the [Logging](./logging) documentation** for detailed logging information
3. **Enable debug logging** to see detailed middleware operations
4. **Create a [minimal code sample](./Minimal-Code-Sample)** that reproduces the issue
5. **Open an issue** on the [GitHub repository](https://github.com/GaProgMan/OwaspHeaders.Core/issues) with:
   - Your configuration code
   - Log output (with debug logging enabled)
   - Expected vs actual behavior
   - Environment details (.NET version, hosting platform)
