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
}
