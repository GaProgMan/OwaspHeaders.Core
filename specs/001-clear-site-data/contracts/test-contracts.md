# Clear-Site-Data Test Contracts

## Unit Test Contracts

### ClearSiteDataConfigurationTests Contract

#### Test Class Structure
```csharp
public class ClearSiteDataConfigurationTests
{
    [Fact] public void BuildHeaderValue_WithWildcard_ReturnsWildcardOnly()
    [Fact] public void BuildHeaderValue_WithMultipleDirectives_ReturnsCommaSeparated()
    [Fact] public void BuildHeaderValue_WithSingleDirective_ReturnsSingleQuoted()
    [Fact] public void BuildHeaderValue_WithWildcardAndOthers_ReturnsWildcardOnly()
    [Fact] public void Constructor_WithNullDirectives_ThrowsArgumentException()
    [Fact] public void Constructor_WithEmptyDirectives_ThrowsArgumentException()
    [Fact] public void Constructor_WithValidDirectives_SetsProperties()
}
```

#### Expected Behaviors
- **Wildcard Precedence**: When `ClearSiteDataOptions.wildcard` present, return `"*"` regardless of other directives
- **Multiple Directives**: Format as `"cache","cookies","storage"` (comma-separated, quoted)
- **Single Directive**: Format as `"cache"` (single quoted string)
- **Validation**: Throw `ArgumentException` for null/empty directive arrays
- **Constructor**: Accept `params ClearSiteDataOptions[]` and store immutably

#### Test Data
```csharp
public static IEnumerable<object[]> ValidDirectiveCombinations()
{
    yield return new object[] { new[] { ClearSiteDataOptions.cache }, "\"cache\"" };
    yield return new object[] { new[] { ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies }, "\"cache\",\"cookies\"" };
    yield return new object[] { new[] { ClearSiteDataOptions.wildcard }, "\"*\"" };
    yield return new object[] { new[] { ClearSiteDataOptions.wildcard, ClearSiteDataOptions.cache }, "\"*\"" };
}
```

### ClearSiteDataPathConfigurationTests Contract

#### Test Class Structure
```csharp
public class ClearSiteDataPathConfigurationTests
{
    [Fact] public void GetConfigurationForPath_ExactMatch_ReturnsConfiguration()
    [Fact] public void GetConfigurationForPath_LongestMatch_ReturnsSpecificConfiguration()
    [Fact] public void GetConfigurationForPath_NoMatch_ReturnsDefault()
    [Fact] public void Constructor_SortsPathsByLength_Descending()
    [Fact] public void Constructor_WithNullPaths_ThrowsArgumentException()
    [Fact] public void Constructor_WithEmptyPath_ThrowsArgumentException()
}
```

#### Expected Behaviors
- **Exact Matching**: `/logout` matches exactly, case-sensitive
- **Longest Match**: `/account/logout` takes precedence over `/logout`
- **No Match**: Return default configuration (may be null)
- **Path Sorting**: Paths sorted by length descending for precedence
- **Validation**: Reject null/empty paths and configurations

#### Test Data
```csharp
public static IEnumerable<object[]> PathPrecedenceScenarios()
{
    yield return new object[] { "/logout", "/account/logout", "/account/logout" }; // Longer wins
    yield return new object[] { "/api/v1/logout", "/logout", "/api/v1/logout" }; // Longer wins
    yield return new object[] { "/admin", "/admin/logout", "/admin/logout" }; // Longer wins
}
```

### ClearSiteDataBuilderTests Contract

#### Test Class Structure
```csharp
public class ClearSiteDataBuilderTests
{
    [Fact] public void UseClearSiteData_WithDefaults_EnablesWithOWASPRecommendations()
    [Fact] public void UseClearSiteData_WithCustomDirectives_UsesSpecifiedDirectives()
    [Fact] public void UseClearSiteDataForPaths_WithValidPaths_ConfiguresCorrectly()
    [Fact] public void AddClearSiteDataPath_ChainedCalls_AddsMultiplePaths()
    [Fact] public void AddClearSiteDataPath_DuplicatePath_UpdatesConfiguration()
    [Fact] public void Build_WithClearSiteData_ReturnsValidConfiguration()
}
```

#### Expected Behaviors
- **Default Configuration**: OWASP recommended directives (`cache`, `cookies`, `storage`)
- **Method Chaining**: All builder methods return configuration for chaining
- **Path Management**: Support adding/updating individual paths fluently
- **Validation**: Reject invalid parameters following existing patterns
- **Integration**: Properly set `UseClearSiteData = true` and configuration objects

## Integration Test Contracts

### ClearSiteDataMiddlewareTests Contract

#### Test Class Structure
```csharp
public class ClearSiteDataMiddlewareTests
{
    [Fact] public async Task InvokeAsync_WithConfiguredPath_AddsCorrectHeader()
    [Fact] public async Task InvokeAsync_WithNonConfiguredPath_AddsNoHeader()
    [Fact] public async Task InvokeAsync_WithPathPrecedence_UsesLongestMatch()
    [Fact] public async Task InvokeAsync_WithWildcardDirective_AddsWildcardHeader()
    [Fact] public async Task InvokeAsync_WithIgnoredUrl_SkipsProcessing()
    [Fact] public async Task InvokeAsync_WithoutClearSiteData_AddsNoHeader()
}
```

#### Expected Behaviors
- **Header Addition**: Correct `Clear-Site-Data` header added for configured paths
- **Path Filtering**: No header for non-configured paths
- **Precedence**: Longer paths take precedence over shorter ones
- **Wildcard**: Wildcard directive generates `"*"` header value
- **Integration**: Works with existing URL ignore functionality
- **Disabled State**: No processing when `UseClearSiteData = false`

#### Test Setup Pattern
```csharp
private TestServer CreateTestServer(SecureHeadersMiddlewareConfiguration config)
{
    return new TestServerBuilder()
        .UseMiddleware<SecureHeadersMiddleware>(config)
        .ConfigureServices(services => services.AddLogging())
        .Build();
}
```

### ClearSiteDataIntegrationTests Contract

#### Test Class Structure
```csharp
public class ClearSiteDataIntegrationTests
{
    [Fact] public async Task EndToEnd_LogoutScenario_ClearsAllData()
    [Fact] public async Task EndToEnd_MultiplePathConfiguration_RespectsAll()
    [Fact] public async Task EndToEnd_WithOtherHeaders_CombinesCorrectly()
    [Fact] public async Task EndToEnd_ThreadSafety_HandlesParallelRequests()
    [Fact] public async Task EndToEnd_PerformanceImpact_MinimalOverhead()
}
```

#### Expected Behaviors
- **Realistic Scenarios**: Test common logout and session clearing scenarios
- **Multi-Path**: Validate multiple path configurations work together
- **Header Combination**: Clear-Site-Data works alongside other security headers
- **Concurrency**: Thread-safe operation under parallel requests
- **Performance**: Minimal impact on request processing time

## Test Data Contracts

### Common Test Scenarios

#### Valid Configuration Examples
```csharp
public static class TestDataSets
{
    public static readonly Dictionary<string, ClearSiteDataOptions[]> LogoutPaths = new()
    {
        ["/logout"] = [ClearSiteDataOptions.wildcard],
        ["/account/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies],
        ["/api/v1/auth/logout"] = [ClearSiteDataOptions.storage]
    };

    public static readonly ClearSiteDataOptions[] OWASPRecommended =
        [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies, ClearSiteDataOptions.storage];
}
```

#### Invalid Configuration Examples
```csharp
public static class InvalidTestData
{
    public static readonly object[] NullDirectives = [null];
    public static readonly object[] EmptyDirectives = [Array.Empty<ClearSiteDataOptions>()];
    public static readonly string[] InvalidPaths = [null, "", "   "];
}
```

### Performance Test Contracts

#### Benchmarking Requirements
```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ClearSiteDataPerformanceBenchmarks
{
    [Benchmark] public void PathResolution_SinglePath()
    [Benchmark] public void PathResolution_MultiplePaths()
    [Benchmark] public void HeaderGeneration_Wildcard()
    [Benchmark] public void HeaderGeneration_MultipleDirectives()
}
```

#### Performance Targets
- **Path Resolution**: <100μs per request
- **Header Generation**: <50μs per header
- **Memory**: Zero allocations per request (after warmup)
- **Throughput**: <1% impact on baseline middleware performance

## Mock and Stub Contracts

### HttpContext Mocking
```csharp
public static class HttpContextMockFactory
{
    public static HttpContext CreateWithPath(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        return context;
    }
}
```

### Configuration Test Helpers
```csharp
public static class ConfigurationTestHelpers
{
    public static SecureHeadersMiddlewareConfiguration CreateWithClearSiteData(
        Dictionary<string, ClearSiteDataOptions[]> paths = null)
    {
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder();

        if (paths != null)
            config.UseClearSiteDataForPaths(paths);
        else
            config.UseClearSiteData();

        return config.Build();
    }
}
```

## Test Validation Contracts

### Assertion Patterns
```csharp
public static class ClearSiteDataAssertions
{
    public static void ShouldHaveClearSiteDataHeader(this HttpResponse response, string expectedValue)
    {
        response.Headers.Should().ContainKey("Clear-Site-Data");
        response.Headers["Clear-Site-Data"].Should().Equal(expectedValue);
    }

    public static void ShouldNotHaveClearSiteDataHeader(this HttpResponse response)
    {
        response.Headers.Should().NotContainKey("Clear-Site-Data");
    }
}
```

### Error Validation
- **ArgumentException**: Validate message content and parameter names
- **Configuration Errors**: Validate logging output and error handling
- **Thread Safety**: Validate no race conditions or deadlocks
- **Performance**: Validate response time targets and memory usage