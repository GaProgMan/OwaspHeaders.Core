# Research: Clear-Site-Data Header Implementation

## W3C/MDN Clear-Site-Data Specification Analysis

**Decision**: Implement Clear-Site-Data header following W3C specification with quoted-string directive format

**Rationale**: W3C/MDN specification provides the authoritative technical requirements including valid directives, syntax, and browser support. This ensures compliance with web standards and maximum browser compatibility.

**Alternatives considered**: Custom implementation rejected because standard compliance is critical for security middleware

**Key Findings**:
- Syntax: `Clear-Site-Data: "directive1", "directive2"` or `"*"` for all
- Valid directives: "cache", "cookies", "storage", "clientHints" (experimental), "executionContexts" (experimental), "*"
- Requires HTTPS (secure context)
- Affects entire registered domain including subdomains
- Baseline 2023 browser support

## OWASP Security Recommendations Analysis

**Decision**: Use OWASP recommended default value `"cache","cookies","storage"` and restrict to stable directives only

**Rationale**: OWASP recommendations prioritize security effectiveness while maintaining broad compatibility. Experimental directives like "clientHints" and "executionContexts" have inconsistent browser support.

**Alternatives considered**: Including experimental directives rejected due to unpredictable behavior and constitution requirement for reliability

**Key Findings**:
- OWASP recommended default: `"cache","cookies","storage"`
- Primary use case: logout security to prevent session hijacking
- Clears DOM storage (localStorage, sessionStorage, IndexedDB), service workers
- Additional processing overhead acceptable for security benefit

## OwaspHeaders.Core Architecture Pattern Analysis

**Decision**: Follow existing header configuration pattern with IConfigurationBase interface and builder methods

**Rationale**: Consistency with existing codebase architecture ensures maintainability and developer familiarity. All security headers follow this pattern.

**Alternatives considered**: New pattern rejected - would violate Library-First Architecture constitutional principle

**Key Implementation Patterns Identified**:
1. Configuration model implements `IConfigurationBase` with `BuildHeaderValue()` method
2. Boolean flag in `SecureHeadersMiddlewareConfiguration` (e.g., `UseClearSiteData`)
3. Builder extension method in `SecureHeadersMiddlewareBuilder` class
4. Header constant in `Constants.cs`
5. Integration in `GenerateRelevantHeaders()` method in middleware
6. Comprehensive test coverage following existing patterns

## Enum-Based Type Safety Pattern Analysis

**Decision**: Use enum with `params` array pattern like `ContentSecurityPolicySandBox` instead of string arrays

**Rationale**: User feedback correctly identified that statically-typed enums prevent spelling errors and follow existing codebase patterns. The `ContentSecurityPolicySandBox(params CspSandboxType[] sandboxType)` pattern allows multiple enum values while maintaining type safety.

**Alternatives considered**:
- `[Flags]` enum rejected - not used in this codebase and adds complexity
- String arrays rejected - no type safety for directive values
- Single enum value rejected - doesn't meet requirement for multiple directives

**Implementation Strategy**:
- Create `ClearSiteDataOptions` enum with stable directives only
- Use `params ClearSiteDataOptions[]` constructor pattern
- Wildcard enum value takes precedence when present (per user requirement)
- BuildHeaderValue() iterates through enum values like CSP sandbox pattern

## Path-Based Configuration Requirements Analysis

**Decision**: Implement path-specific configuration using Dictionary<string, ClearSiteDataOptions[]> for exact path matching

**Rationale**: Feature specification requires Clear-Site-Data header only on explicit paths (like /logout). Current middleware applies headers globally - need path-specific mechanism with typed enum values.

**Alternatives considered**: Global application rejected - violates functional requirement FR-001 for path-specific configuration

**Technical Approach**:
- Extend middleware to check request path against configured Clear-Site-Data paths
- Use exact string matching (no wildcards) per FR-006
- Path takes precedence over global header configuration
- Store enum arrays per path for type safety
- Maintain thread-safe configuration access

## ClearSiteDataOptions Enum Design Analysis

**Decision**: Define enum with stable directives only, following naming convention of existing enums

**Rationale**: Existing enums use exact specification names (lowercased) for direct string conversion without "C# string magic". Experimental directives excluded for stability.

**Enum Design**:
```csharp
public enum ClearSiteDataOptions
{
    cache,
    cookies,
    storage,
    wildcard  // represents "*" - takes precedence per user requirement
}
```

**Alternatives considered**:
- Including experimental directives rejected - browser support inconsistency
- CamelCase naming rejected - violates existing enum naming patterns
- Separate "all" enum rejected - wildcard covers this use case

## Wildcard Precedence Logic Analysis

**Decision**: Check for wildcard enum presence first, return `"*"` immediately if found

**Rationale**: User requirement that wildcard takes precedence over specific directives. Most efficient implementation checks for wildcard first.

**Implementation Strategy**:
```csharp
public string BuildHeaderValue()
{
    if (DirectiveOptions.Contains(ClearSiteDataOptions.wildcard))
    {
        return "\"*\"";
    }
    // Process other directives...
}
```

**Alternatives considered**: Post-processing replacement rejected - less efficient than early detection

## Path Precedence Strategy Analysis

**Decision**: Implement longest-match path precedence using OrderByDescending on path length

**Rationale**: Feature specification edge case requires more specific paths to take precedence (e.g., `/account/logout` over `/logout`). Longest-match is standard and predictable.

**Alternatives considered**: First-match rejected - would be unpredictable for developers

**Implementation Strategy**:
- Sort configured paths by length (descending) during configuration
- Use first match during request processing
- Document precedence behavior clearly in builder method

## Error Handling Pattern Analysis

**Decision**: Follow existing ArgumentException pattern with HeaderValueGuardClauses for validation

**Rationale**: Constitutional requirement for consistency. All existing headers use this pattern for invalid configurations.

**Alternatives considered**: Custom exceptions rejected - would violate existing patterns

**Validation Requirements**:
- Non-null/empty directive arrays using existing guard clauses
- Path strings cannot be null/empty
- Use existing `HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace` pattern
- Enum validation handled by compiler (type safety benefit)

## Thread Safety Analysis

**Decision**: Use FrozenDictionary for path configurations and immutable directive arrays

**Rationale**: Middleware must be thread-safe as per existing architecture. Configuration is built once during startup.

**Alternatives considered**: Concurrent collections rejected - unnecessary overhead for read-only post-configuration data

**Thread Safety Approach**:
- Path configuration frozen during Build()
- Directive arrays immutable after validation
- No runtime modification of configuration
- Follow existing _headers FrozenDictionary pattern

## Performance Impact Analysis

**Decision**: Minimal overhead approach with O(1) path lookups and pre-generated header values

**Rationale**: Constitutional performance requirement for minimal middleware overhead (<1ms processing).

**Performance Strategy**:
- Pre-generate all header values during configuration build
- Use FrozenDictionary for O(1) average case path lookups
- Cache header strings to avoid string concatenation per request
- Early exit for requests not matching any configured paths
- Wildcard early-exit optimization

## Testing Strategy Analysis

**Decision**: Follow existing xUnit test patterns with dedicated test files for each component

**Rationale**: Constitutional Test-Driven Development requirement mandates 90%+ coverage for security-critical paths.

**Test Coverage Plan**:
- ClearSiteDataConfigurationTests.cs - Configuration model validation and enum handling
- ClearSiteDataBuilderTests.cs - Builder method functionality with enum parameters
- ClearSiteDataMiddlewareTests.cs - Path matching and header generation
- ClearSiteDataIntegrationTests.cs - End-to-end scenarios with various enum combinations
- Wildcard precedence testing for enum combinations
- Edge case testing for path precedence and invalid configurations