# OwaspHeaders.Core Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-09-20

## Active Technologies
- C# .NET (multiple target frameworks) + ASP.NET Core middleware pipeline (001-clear-site-data)

## Project Structure
```
src/
├── Enums/           # Security header enums (e.g., ClearSiteDataOptions)
├── Models/          # Configuration models implementing IConfigurationBase
├── Extensions/      # Builder extensions and middleware extensions
├── Guards/          # Validation guard clauses
├── Helpers/         # Utility helpers
└── Constants.cs     # Header name constants
tests/
├── CustomHeaders/   # Header-specific tests
├── GuardClauses/    # Validation tests
└── LoggingTests/    # Logging functionality tests
```

## Commands
- `dotnet test` - Run all tests
- `dotnet format` - Format code (required for CI/CD)
- `dotnet build` - Build all projects

## Code Style
- Follow existing enum naming conventions (lowercase, match header spec exactly)
- Use `params` arrays for multiple directive support (like ContentSecurityPolicySandBox)
- Implement `IConfigurationBase` with `BuildHeaderValue()` method
- Use existing `HeaderValueGuardClauses` for validation
- Follow builder pattern with method chaining
- Maintain thread safety with frozen collections post-build
- Add XML documentation for all public APIs

## Recent Changes
- 001-clear-site-data: Added Clear-Site-Data header support with enum-based type safety, path-specific configuration, and OWASP compliance

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->