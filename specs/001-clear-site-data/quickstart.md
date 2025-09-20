# Clear-Site-Data Header Quickstart Guide

## Overview
This guide demonstrates how to configure and test the Clear-Site-Data header implementation in OwaspHeaders.Core. The Clear-Site-Data header instructs browsers to clear cached data, cookies, and storage when users access specific endpoints like logout pages.

## Basic Usage Examples

### 1. Enable Clear-Site-Data with OWASP Defaults
```csharp
using OwaspHeaders.Core.Extensions;

// Configure middleware with OWASP recommended directives
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteData() // Defaults to "cache","cookies","storage"
    .Build();

// Add to ASP.NET Core pipeline
app.UseMiddleware<SecureHeadersMiddleware>(config);
```

**Expected Result**: All responses include `Clear-Site-Data: "cache","cookies","storage"`

### 2. Custom Directives for All Requests
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteData(ClearSiteDataOptions.wildcard) // Clear everything
    .Build();
```

**Expected Result**: All responses include `Clear-Site-Data: "*"`

### 3. Path-Specific Configuration (Recommended)
```csharp
var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
{
    ["/logout"] = [ClearSiteDataOptions.wildcard],
    ["/account/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies],
    ["/api/auth/signout"] = [ClearSiteDataOptions.storage]
};

var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteDataForPaths(pathConfig)
    .Build();
```

**Expected Result**:
- `/logout` → `Clear-Site-Data: "*"`
- `/account/logout` → `Clear-Site-Data: "cache","cookies"`
- `/api/auth/signout` → `Clear-Site-Data: "storage"`
- Other paths → No Clear-Site-Data header

### 4. Fluent Path Configuration
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
    .AddClearSiteDataPath("/admin/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
    .AddClearSiteDataPath("/mobile/logout", ClearSiteDataOptions.cookies)
    .Build();
```

## Testing Your Implementation

### 1. Unit Test: Configuration Validation
```csharp
[Fact]
public void Configuration_WithValidPaths_BuildsCorrectly()
{
    // Arrange
    var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
    {
        ["/logout"] = [ClearSiteDataOptions.wildcard]
    };

    // Act
    var config = SecureHeadersMiddlewareBuilder
        .CreateBuilder()
        .UseClearSiteDataForPaths(pathConfig)
        .Build();

    // Assert
    config.UseClearSiteData.Should().BeTrue();
    config.ClearSiteDataPathConfiguration.Should().NotBeNull();
}
```

### 2. Integration Test: Header Validation
```csharp
[Fact]
public async Task Middleware_OnLogoutPath_AddsCorrectHeader()
{
    // Arrange
    var config = SecureHeadersMiddlewareBuilder
        .CreateBuilder()
        .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
        .Build();

    using var server = new TestServerBuilder()
        .UseMiddleware<SecureHeadersMiddleware>(config)
        .Build();

    // Act
    var response = await server.CreateClient().GetAsync("/logout");

    // Assert
    response.Headers.Should().ContainKey("Clear-Site-Data");
    response.Headers.GetValues("Clear-Site-Data").First().Should().Be("\"*\"");
}
```

### 3. Browser Testing
```bash
# Test with curl to verify header output
curl -I https://yourapp.com/logout

# Expected response includes:
# Clear-Site-Data: "*"
```

## Path Precedence Examples

### Scenario: Multiple Logout Endpoints
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
    .AddClearSiteDataPath("/account/logout", ClearSiteDataOptions.wildcard) // More specific
    .Build();
```

**Test Cases**:
- `/logout` → `Clear-Site-Data: "cache","cookies"`
- `/account/logout` → `Clear-Site-Data: "*"` (longer path wins)
- `/account/settings` → No header

### Verification Test
```csharp
[Theory]
[InlineData("/logout", "\"cache\",\"cookies\"")]
[InlineData("/account/logout", "\"*\"")]
[InlineData("/account/settings", null)]
public async Task PathPrecedence_ResolvesCorrectly(string path, string expectedHeader)
{
    // Test implementation validates precedence rules
}
```

## Common Scenarios

### 1. Session Logout Security
```csharp
// Maximum security for user logout
.AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
.AddClearSiteDataPath("/signout", ClearSiteDataOptions.wildcard)
```

### 2. API Endpoint Cleanup
```csharp
// Clear specific data types for API endpoints
.AddClearSiteDataPath("/api/auth/logout",
    ClearSiteDataOptions.cache,
    ClearSiteDataOptions.cookies)
```

### 3. Administrative Actions
```csharp
// Clear cache for admin operations
.AddClearSiteDataPath("/admin/clear-cache", ClearSiteDataOptions.cache)
.AddClearSiteDataPath("/admin/logout", ClearSiteDataOptions.wildcard)
```

## Validation Checklist

### ✅ Configuration Validation
- [ ] Directives contain at least one valid option
- [ ] Path strings are not null, empty, or whitespace
- [ ] Wildcard precedence works correctly
- [ ] Path precedence follows longest-match rule

### ✅ Runtime Validation
- [ ] Correct headers added for configured paths
- [ ] No headers added for non-configured paths
- [ ] Thread-safe operation under concurrent requests
- [ ] Performance impact <1ms per request

### ✅ Security Validation
- [ ] OWASP recommended defaults used appropriately
- [ ] Wildcard directive clears all client data
- [ ] Path-specific configurations work as expected
- [ ] Integration with existing security headers

## Troubleshooting

### Issue: Header Not Appearing
```csharp
// Verify middleware is enabled
config.UseClearSiteData.Should().BeTrue();

// Verify path configuration
var pathConfig = config.ClearSiteDataPathConfiguration;
pathConfig.GetConfigurationForPath("/logout").Should().NotBeNull();
```

### Issue: Wrong Header Value
```csharp
// Test header generation directly
var clearSiteDataConfig = new ClearSiteDataConfiguration(ClearSiteDataOptions.wildcard);
var headerValue = clearSiteDataConfig.BuildHeaderValue();
headerValue.Should().Be("\"*\"");
```

### Issue: Path Not Matching
```csharp
// Verify exact path matching (case-sensitive)
"/Logout" != "/logout" // Different cases don't match
"/logout/" != "/logout" // Trailing slash matters
```

## Performance Validation

### Benchmark Test
```csharp
[Fact]
public async Task Performance_WithClearSiteData_MinimalOverhead()
{
    var stopwatch = Stopwatch.StartNew();

    // Process 1000 requests
    for (int i = 0; i < 1000; i++)
    {
        await server.CreateClient().GetAsync("/logout");
    }

    stopwatch.Stop();
    var averageTime = stopwatch.ElapsedMilliseconds / 1000.0;

    // Should be less than 1ms per request
    averageTime.Should().BeLessThan(1.0);
}
```

## Integration with Other Headers

### Complete Security Configuration
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .UseDefaultContentSecurityPolicy()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
    .Build();
```

**Expected Result**: All security headers work together without conflicts.

## Next Steps

1. **Implement Core Classes**: Create the enum, configuration, and builder classes
2. **Add Middleware Integration**: Extend existing middleware for path-specific processing
3. **Write Comprehensive Tests**: Cover all scenarios and edge cases
4. **Performance Testing**: Validate minimal overhead requirements
5. **Documentation**: Update XML documentation and examples
6. **Integration Testing**: Test with real applications and various frameworks