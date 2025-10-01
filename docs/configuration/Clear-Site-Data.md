---
title: Clear-Site-Data
nav_order: 11
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Clear-Site-Data header like this:

{: .quote }
> The HTTP Clear-Site-Data response header clears browsing data (cookies, storage, cache) associated with the requesting website. It allows web developers to have more control over the data stored locally by a browser for their origins.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Clear-Site-Data

{: .warning }
Clear-Site-Data is **not** included in the default middleware configuration due to its potentially disruptive nature. It must be explicitly configured.

## Basic Usage

### Global Configuration

Add Clear-Site-Data to all responses with OWASP recommended defaults:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteData() // Defaults to "cache","cookies","storage"
    .Build();

app.UseSecureHeadersMiddleware(config);
```

The above adds the Clear-Site-Data header with `"cache","cookies","storage"` value to all responses.

### Path-Specific Configuration (Recommended)

Configure Clear-Site-Data for specific paths like logout endpoints:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
    .AddClearSiteDataPath("/api/auth/signout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

The above configuration will:
- Add `Clear-Site-Data: "*"` to responses for `/logout`
- Add `Clear-Site-Data: "cache","cookies"` to responses for `/api/auth/signout`
- No Clear-Site-Data header for other paths

### Bulk Path Configuration

Configure multiple paths at once:

{: .note }
The following example uses [C# 12's collection expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions). The more established `[key] = value` style is also supported.

```csharp
var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
{
    ["/logout"] = [ClearSiteDataOptions.wildcard],
    ["/account/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies],
    ["/admin/logout"] = [ClearSiteDataOptions.storage]
};

var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteDataForPaths(pathConfig)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

## Directive Options

The Clear-Site-Data header supports the following directive options via the `ClearSiteDataOptions` enum:

### Available Options

- `cache` - Clears browser cache for the origin
- `cookies` - Clears all cookies for the origin
- `storage` - Clears DOM storage (localStorage, sessionStorage, IndexedDB) for the origin
- `wildcard` - Represents "*" - clears all data types (takes precedence over other directives)

{: .note }
**Experimental Directives Not Supported**: This implementation intentionally excludes experimental directive values such as `clientHints` and `executionContexts`. These directives are not yet stable across browser implementations and may change. Only the stable, well-supported directive values are included to ensure consistent behavior across all supported browsers.

### Option Behavior

- **Wildcard Precedence**: If `wildcard` is included with other options, it takes precedence and results in `Clear-Site-Data: "*"`
- **Duplicate Handling**: Duplicate options are automatically deduplicated
- **OWASP Defaults**: When no options are specified, defaults to `cache`, `cookies`, and `storage`

## Common Use Cases

### Maximum Security Logout

Clear all client-side data on logout for maximum security:

```csharp
.AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
.AddClearSiteDataPath("/signout", ClearSiteDataOptions.wildcard)
```

Results in: `Clear-Site-Data: "*"`

### Selective Data Clearing

Clear only specific data types:

```csharp
.AddClearSiteDataPath("/api/auth/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
```

Results in: `Clear-Site-Data: "cache","cookies"`

### Administrative Actions

Clear cache for admin operations:

```csharp
.AddClearSiteDataPath("/admin/clear-cache", ClearSiteDataOptions.cache)
.AddClearSiteDataPath("/admin/logout", ClearSiteDataOptions.wildcard)
```

## Path Matching Rules

### Exact Matching

Clear-Site-Data uses **exact path matching** (case-sensitive):

- `/logout` matches `/logout` exactly
- `/logout` does **not** match `/Logout` (different case)
- `/logout` does **not** match `/logout/` (trailing slash matters)

### Path Precedence

When multiple paths could match, the **longest path wins**:

```csharp
var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
{
    ["/account"] = [ClearSiteDataOptions.cache],
    ["/account/logout"] = [ClearSiteDataOptions.wildcard] // This wins for /account/logout
};
```

For request `/account/logout`, the longer path `/account/logout` takes precedence over `/account`.

## Full Configuration Example

Here's a comprehensive example showing Clear-Site-Data integration with other security headers:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .UseDefaultContentSecurityPolicy()
    .UsePermittedCrossDomainPolicies()
    .UseReferrerPolicy()
    .UseCacheControl()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
    .AddClearSiteDataPath("/api/auth/signout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
    .AddClearSiteDataPath("/admin/logout", ClearSiteDataOptions.wildcard)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

## Security Considerations

### When to Use Clear-Site-Data

- **Logout endpoints**: Clear all data to prevent session hijacking
- **Authentication changes**: Clear sensitive cached data after privilege changes
- **Security incidents**: Force data clearing during incident response
- **Administrative actions**: Clear cache after configuration changes

### Performance Impact

- **Minimal overhead**: <1ms processing time per request
- **Client-side impact**: Browsers may need to re-download cached resources
- **Storage clearing**: May affect user experience if used inappropriately

### Browser Support

Clear-Site-Data is supported by modern browsers. Check [MDN compatibility data](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Clear-Site-Data#browser_compatibility) for current support status.

## Best Practices

1. **Use path-specific configuration** rather than global configuration
2. **Apply to logout endpoints** for maximum security
3. **Use wildcard for high-security scenarios** like logout
4. **Use selective clearing** for performance-sensitive scenarios
5. **Test thoroughly** as it affects client-side data
6. **Document usage** for your team to understand the impact

## Migration Notes

If upgrading from a version without Clear-Site-Data support:

- Clear-Site-Data is **not enabled by default**
- No breaking changes to existing configurations
- Explicit configuration required to enable the feature
- Path-specific configuration is the recommended approach