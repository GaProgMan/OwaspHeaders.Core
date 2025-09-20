# Clear-Site-Data Builder API Contract

## UseClearSiteData Extension Method

### Method Signature
```csharp
public static SecureHeadersMiddlewareConfiguration UseClearSiteData(
    this SecureHeadersMiddlewareConfiguration config,
    ClearSiteDataOptions defaultDirectives = ClearSiteDataOptions.cache | ClearSiteDataOptions.cookies | ClearSiteDataOptions.storage)
```

### Purpose
Enables Clear-Site-Data header for all requests using specified default directives.

### Parameters
- `config`: The middleware configuration to extend (required)
- `defaultDirectives`: Default directives to use for all requests (optional, defaults to OWASP recommendations)

### Return Value
- Returns the same `SecureHeadersMiddlewareConfiguration` instance for method chaining

### Behavior
- Sets `config.UseClearSiteData = true`
- Creates default `ClearSiteDataPathConfiguration` with global configuration
- Validates directive parameters using existing guard clause patterns
- Follows existing builder method patterns for consistency

### Validation Rules
- Throws `ArgumentException` if config is null
- Validates that at least one directive is specified
- Uses existing `HeaderValueGuardClauses` for parameter validation

### Usage Example
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteData() // Uses OWASP defaults
    .Build();
```

## UseClearSiteDataForPaths Extension Method

### Method Signature
```csharp
public static SecureHeadersMiddlewareConfiguration UseClearSiteDataForPaths(
    this SecureHeadersMiddlewareConfiguration config,
    Dictionary<string, ClearSiteDataOptions[]> pathConfigurations)
```

### Purpose
Enables Clear-Site-Data header for specific paths with custom directive configurations.

### Parameters
- `config`: The middleware configuration to extend (required)
- `pathConfigurations`: Dictionary mapping paths to directive arrays (required)

### Return Value
- Returns the same `SecureHeadersMiddlewareConfiguration` instance for method chaining

### Behavior
- Sets `config.UseClearSiteData = true`
- Creates `ClearSiteDataPathConfiguration` with specified path mappings
- Validates all paths and directive configurations
- Sorts paths by length for precedence resolution

### Validation Rules
- Throws `ArgumentException` if pathConfigurations is null or empty
- Validates each path key is not null/empty/whitespace
- Validates each directive array is not null or empty
- Ensures path keys are unique (case-sensitive)

### Usage Example
```csharp
var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
{
    ["/logout"] = [ClearSiteDataOptions.wildcard],
    ["/account/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies]
};

var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseClearSiteDataForPaths(pathConfig)
    .Build();
```

## AddClearSiteDataPath Extension Method

### Method Signature
```csharp
public static SecureHeadersMiddlewareConfiguration AddClearSiteDataPath(
    this SecureHeadersMiddlewareConfiguration config,
    string path,
    params ClearSiteDataOptions[] directives)
```

### Purpose
Adds or updates Clear-Site-Data configuration for a specific path (fluent API).

### Parameters
- `config`: The middleware configuration to extend (required)
- `path`: The exact path to configure (required)
- `directives`: The directive options for this path (required, params array)

### Return Value
- Returns the same `SecureHeadersMiddlewareConfiguration` instance for method chaining

### Behavior
- Initializes Clear-Site-Data if not already enabled
- Adds or updates path configuration in existing `ClearSiteDataPathConfiguration`
- Validates path and directives
- Maintains path precedence sorting

### Validation Rules
- Throws `ArgumentException` if path is null/empty/whitespace
- Throws `ArgumentException` if directives array is null or empty
- Path matching is case-sensitive and exact

### Usage Example
```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
    .AddClearSiteDataPath("/api/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
    .Build();
```

## Error Handling Contract

### ArgumentException Scenarios
1. **Null configuration object**: "Configuration cannot be null"
2. **Empty directive array**: "At least one Clear-Site-Data directive must be specified"
3. **Null/empty path**: "Path cannot be null, empty, or whitespace"
4. **Empty path configurations**: "Path configurations dictionary cannot be null or empty"

### Exception Message Format
All exceptions follow existing pattern using `ArgumentExceptionHelper.RaiseException()` or similar existing utilities.

### Guard Clause Integration
Uses existing `HeaderValueGuardClauses` and `ObjectGuardClauses` for consistency:
- `HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace()`
- `ObjectGuardClauses.ObjectCannotBeNull()`

## Thread Safety Contract

### Immutable Configuration
- All configuration modifications occur during builder phase
- Runtime configuration is read-only and thread-safe
- Path configurations frozen after Build() call

### Concurrent Access
- Multiple threads can safely read configuration
- No runtime modification after middleware initialization
- Thread-safe dictionary access patterns

## Performance Contract

### Builder Performance
- O(1) for single path additions
- O(n log n) for path sorting during Build()
- Minimal memory allocations during configuration

### Runtime Performance
- O(1) average case for path lookup
- O(n) worst case for path precedence resolution (n = configured paths)
- Pre-computed header values where possible

## Backward Compatibility Contract

### Existing API Preservation
- No changes to existing builder methods
- No changes to existing configuration properties
- New properties use default values that don't affect existing behavior

### Default Behavior
- Clear-Site-Data disabled by default (`UseClearSiteData = false`)
- No headers added unless explicitly configured
- Existing security headers unaffected