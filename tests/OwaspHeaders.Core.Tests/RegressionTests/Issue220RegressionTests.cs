namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// Regression tests for https://github.com/GaProgMan/OwaspHeaders.Core/issues/220
///
/// Setting a <c>UseX</c> flag on the configuration without going through the matching
/// builder extension used to surface as an unhandled <see cref="NullReferenceException"/>
/// on the first request. The middleware now validates the configuration on first invoke
/// and throws a diagnostic <see cref="ArgumentException"/> listing every mismatch.
/// </summary>
public class Issue220RegressionTests
{
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public Issue220RegressionTests()
    {
        _onNext = _ => Task.CompletedTask;
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task FlagSetWithoutBuilder_Throws_ArgumentException_NamingTheFlag()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseReferrerPolicy()
            .Build();

        // bypass the builder — this is the exact scenario from issue #220
        config.UseCacheControl = true;

        var middleware = new SecureHeadersMiddleware(_onNext, config);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        var argEx = Assert.IsAssignableFrom<ArgumentException>(exception);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseCacheControl), argEx.Message);
    }

    [Fact]
    public async Task MultipleFlagsSetWithoutBuilder_AllReportedInSingleMessage()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .Build();

        config.UseCacheControl = true;
        config.UseHsts = true;
        config.UseXFrameOptions = true;

        var middleware = new SecureHeadersMiddleware(_onNext, config);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        var argEx = Assert.IsAssignableFrom<ArgumentException>(exception);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseCacheControl), argEx.Message);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseHsts), argEx.Message);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseXFrameOptions), argEx.Message);
    }

    [Fact]
    public async Task ValidBuilderConfiguration_DoesNotThrow()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseCacheControl()
            .UseReferrerPolicy()
            .Build();

        var middleware = new SecureHeadersMiddleware(_onNext, config);

        var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(_context));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_OnConfigBuiltViaBuilder_ReturnsNoIssues()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseCacheControl()
            .Build();

        var issues = config.Validate();

        Assert.Empty(issues);
    }

    [Fact]
    public void Validate_FlagWithoutConfig_ReportsExactlyOneIssue()
    {
        var config = new SecureHeadersMiddlewareConfiguration { UseCacheControl = true };

        var issues = config.Validate();

        Assert.Single(issues);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseCacheControl), issues[0]);
    }

    /// <summary>
    /// Reflection-driven coverage check: every <c>public bool Use*</c> property on
    /// <see cref="SecureHeadersMiddlewareConfiguration"/> must either be reported by
    /// <see cref="SecureHeadersMiddlewareConfiguration.Validate"/> when enabled without a
    /// backing configuration, or be listed in the allowlist below as a flag that
    /// intentionally has no backing configuration object.
    /// </summary>
    /// <remarks>
    /// This guards against two regressions: a contributor removing a <c>Check(...)</c>
    /// call from <c>Validate</c>, and a contributor adding a new <c>UseX</c> flag without
    /// extending <c>Validate</c>. In both cases the failure message names the offending
    /// flag and points at the fix.
    /// </remarks>
    [Fact]
    public void Validate_ReportsEveryUseFlagThatHasABackingConfiguration()
    {
        // Flags that emit a constant header value and have no backing configuration
        // object. Add to this set only when introducing a header that follows the same
        // pattern (e.g. X-Content-Type-Options always emits "nosniff").
        var flagsWithoutBackingConfiguration = new HashSet<string>
        {
            nameof(SecureHeadersMiddlewareConfiguration.UseXContentTypeOptions),
        };

        var useFlags = typeof(SecureHeadersMiddlewareConfiguration)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(bool)
                        && p.Name.StartsWith("Use", StringComparison.Ordinal)
                        && p.CanWrite)
            .ToList();

        Assert.NotEmpty(useFlags);

        foreach (var flag in useFlags)
        {
            if (flagsWithoutBackingConfiguration.Contains(flag.Name))
            {
                continue;
            }

            var config = new SecureHeadersMiddlewareConfiguration();
            flag.SetValue(config, true);

            var issues = config.Validate();

            Assert.True(
                issues.Any(i => i.Contains(flag.Name, StringComparison.Ordinal)),
                $"Validate() did not report an issue for {flag.Name}. " +
                $"If this is a new flag, add a matching Check(...) call in " +
                $"SecureHeadersMiddlewareConfiguration.Validate(). " +
                $"If it intentionally has no backing configuration object, add it to " +
                $"the flagsWithoutBackingConfiguration set in this test.");
        }
    }
}
