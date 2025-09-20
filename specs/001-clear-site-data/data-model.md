# Data Model: Clear-Site-Data Header Implementation

## Primary Entities

### ClearSiteDataOptions (Enum)
**Purpose**: Type-safe enumeration of valid Clear-Site-Data directive values
**Location**: `src/Enums/ClearSiteDataOptions.cs`

```csharp
public enum ClearSiteDataOptions
{
    cache,      // Clears browser cache for origin
    cookies,    // Clears all cookies for origin
    storage,    // Clears DOM storage (localStorage, sessionStorage, IndexedDB)
    wildcard    // Represents "*" - clears all data types (takes precedence)
}
```

**Validation Rules**:
- Enum provides compile-time type safety
- Only stable directives included (no experimental clientHints/executionContexts)
- Wildcard enum value takes precedence over all other values when present
- Direct string mapping without transformation ("cache" → "cache", "wildcard" → "*")

**Relationships**:
- Used by `ClearSiteDataConfiguration` to store directive options
- Consumed by `BuildHeaderValue()` method for header string generation

### ClearSiteDataConfiguration (Model)
**Purpose**: Configuration model for Clear-Site-Data header behavior
**Location**: `src/Models/ClearSiteDataConfiguration.cs`
**Implements**: `IConfigurationBase`

```csharp
public class ClearSiteDataConfiguration : IConfigurationBase
{
    public ClearSiteDataOptions[] DirectiveOptions { get; init; }

    protected ClearSiteDataConfiguration() { }

    public ClearSiteDataConfiguration(params ClearSiteDataOptions[] directiveOptions)
    {
        // Validation logic
        DirectiveOptions = directiveOptions;
    }

    public string BuildHeaderValue()
    {
        // Wildcard precedence logic + directive processing
    }
}
```

**Validation Rules**:
- DirectiveOptions array cannot be null or empty
- Duplicate enum values are allowed but ignored during processing
- Wildcard presence triggers immediate return of `"*"`
- Non-wildcard directives formatted as comma-separated quoted strings

**State Transitions**:
- Immutable after construction (init-only properties)
- Thread-safe for middleware usage
- Validation occurs during construction

### ClearSiteDataPathConfiguration (Model)
**Purpose**: Path-specific Clear-Site-Data configuration mapping
**Location**: `src/Models/ClearSiteDataPathConfiguration.cs`

```csharp
public class ClearSiteDataPathConfiguration
{
    public Dictionary<string, ClearSiteDataConfiguration> PathConfigurations { get; init; }
    public ClearSiteDataConfiguration DefaultConfiguration { get; init; }

    public ClearSiteDataPathConfiguration(
        Dictionary<string, ClearSiteDataConfiguration> pathConfigurations,
        ClearSiteDataConfiguration defaultConfiguration = null)
    {
        // Validation and sorting logic
    }

    public ClearSiteDataConfiguration GetConfigurationForPath(string requestPath)
    {
        // Longest-match path resolution logic
    }
}
```

**Validation Rules**:
- Path keys cannot be null, empty, or whitespace
- Path keys must be exact matches (no wildcards or regex)
- Configurations cannot be null
- Paths sorted by length (descending) for precedence resolution

**Relationships**:
- Contains multiple `ClearSiteDataConfiguration` instances
- Used by middleware to resolve path-specific configurations
- Frozen after construction for thread safety

## Integration Entities

### SecureHeadersMiddlewareConfiguration (Extension)
**Purpose**: Add Clear-Site-Data support to existing middleware configuration
**Location**: `src/Models/SecureHeadersMiddlewareConfiguration.cs`

**New Properties**:
```csharp
// Add to existing class
public bool UseClearSiteData { get; set; }
public ClearSiteDataPathConfiguration ClearSiteDataPathConfiguration { get; set; }
```

**Validation Rules**:
- `UseClearSiteData` flag controls header inclusion
- `ClearSiteDataPathConfiguration` can be null (no Clear-Site-Data headers)
- Boolean flag follows existing pattern of other security headers

### Constants (Extension)
**Purpose**: Add Clear-Site-Data header name constant
**Location**: `src/Constants.cs`

**New Constant**:
```csharp
public const string ClearSiteDataHeaderName = "Clear-Site-Data";
```

## Header Value Generation Logic

### Wildcard Precedence
```
IF DirectiveOptions contains ClearSiteDataOptions.wildcard:
    RETURN "\"*\""
ELSE:
    Process individual directives
```

### Individual Directive Processing
```
FOR EACH directive IN DirectiveOptions (excluding wildcard):
    SWITCH directive:
        cache → "\"cache\""
        cookies → "\"cookies\""
        storage → "\"storage\""
    JOIN with comma separator
RETURN joined string
```

### Path Resolution Logic
```
FOR EACH path IN PathConfigurations (sorted by length DESC):
    IF requestPath.Equals(path, StringComparison.InvariantCulture):
        RETURN associated configuration
RETURN DefaultConfiguration (may be null)
```

## Thread Safety Guarantees

### Immutable Configuration
- All configuration objects use `init` properties
- Arrays stored as immutable after validation
- Dictionary frozen after construction

### Thread-Safe Access
- No runtime modification of configuration state
- Read-only operations in middleware request processing
- Pre-computed header values where possible

## Validation Error Handling

### Construction Validation
- Throw `ArgumentException` for null/empty directive arrays
- Throw `ArgumentException` for null/empty path strings
- Follow existing `HeaderValueGuardClauses` patterns

### Runtime Safety
- Enum values provide compile-time safety
- Path matching uses safe string comparison
- Default/fallback behavior for missing configurations

## Performance Characteristics

### Memory Usage
- Enum arrays have minimal overhead
- Dictionary lookup O(1) average case
- Pre-computed header strings reduce allocation

### Processing Time
- Early wildcard detection avoids unnecessary processing
- Sorted path lookup optimizes common cases
- Single string concatenation for multi-directive headers

## Testing Data Requirements

### Unit Test Scenarios
- Single directive configurations
- Multiple directive combinations
- Wildcard precedence validation
- Path precedence resolution
- Invalid configuration rejection

### Integration Test Data
- Logout endpoint configurations (`/logout`, `/account/logout`)
- Wildcard vs specific directive combinations
- Path collision scenarios
- Thread safety validation with concurrent requests