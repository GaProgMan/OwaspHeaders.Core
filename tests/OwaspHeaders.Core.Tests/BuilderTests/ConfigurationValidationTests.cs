namespace OwaspHeaders.Core.Tests.BuilderTests;

/// <summary>
/// Covers configuration validation and, in particular, *when* it happens: at the point the
/// middleware is constructed, which ASP.NET Core does while building the request pipeline.
/// </summary>
public class ConfigurationValidationTests
{
    private readonly RequestDelegate _onNext = _ => Task.CompletedTask;

    [Fact]
    public void ValidateOrThrow_OnAValidConfiguration_DoesNotThrow()
    {
        // arrange
        var config = new SecureHeadersBuilder().UseRecommendedDefaults().Build();

        // act
        var exception = Record.Exception(() => config.ValidateOrThrow());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateOrThrow_OnAnInvalidConfiguration_ThrowsNamingTheFlag()
    {
        // arrange
        var config = new SecureHeadersBuilder().Build();
        config.UseCacheControl = true;

        // act
        var exception = Record.Exception(() => config.ValidateOrThrow());

        // assert
        Assert.NotNull(exception);
        var argEx = Assert.IsAssignableFrom<ArgumentException>(exception);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseCacheControl), argEx.Message);
    }

    [Fact]
    public void Validate_ReportsCrossOriginEmbedderPolicyWithoutResourcePolicy()
    {
        // arrange
        var config = new SecureHeadersBuilder().UseCrossOriginEmbedderPolicy().Build();

        // act
        var issues = config.Validate();

        // assert
        // This is a cross-header rule rather than a missing configuration object, and it used to
        // be enforced from inside header generation, i.e. on the first request.
        var issue = Assert.Single(issues);
        Assert.Contains("Cross-Origin-Embedder-Policy requires Cross-Origin-Resource-Policy", issue);
    }

    [Fact]
    public void Validate_CrossOriginEmbedderPolicyWithResourcePolicy_ReportsNothing()
    {
        // arrange
        var config = new SecureHeadersBuilder()
            .UseCrossOriginResourcePolicy()
            .UseCrossOriginEmbedderPolicy()
            .Build();

        // act, assert
        Assert.Empty(config.Validate());
    }

    [Fact]
    public void Validate_CrossOriginEmbedderPolicyFlagWithoutItsConfiguration_DoesNotThrow()
    {
        // arrange
        // The flag is set without the matching configuration object, so the cross-header rule
        // must not dereference it. Validate exists to report this, not to throw from inside it.
        var config = new SecureHeadersBuilder().Build();
        config.UseCrossOriginEmbedderPolicy = true;

        // act
        var issues = config.Validate();

        // assert
        var issue = Assert.Single(issues);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration.UseCrossOriginEmbedderPolicy), issue);
    }

    [Fact]
    public void ConstructingTheMiddleware_WithAnInvalidConfiguration_Throws()
    {
        // arrange
        var config = new SecureHeadersBuilder().Build();
        config.UseHsts = true;

        // act
        var exception = Record.Exception(() => new SecureHeadersMiddleware(_onNext, config));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);
    }

    [Fact]
    public async Task ConstructingTheMiddleware_WithAValidConfiguration_ServesRequests()
    {
        // arrange
        var config = new SecureHeadersBuilder().UseRecommendedDefaults().Build();
        var middleware = new SecureHeadersMiddleware(_onNext, config);
        var context = new DefaultHttpContext();

        // act
        await middleware.InvokeAsync(context);

        // assert
        Assert.True(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public void BuildAndValidate_WithAValidDelegate_ReturnsTheConfiguration()
    {
        // arrange, act
        var config = SecureHeadersBuilder.BuildAndValidate(opt => opt.UseRecommendedDefaults());

        // assert
        Assert.NotNull(config);
        Assert.True(config.UseHsts);
    }

    [Fact]
    public void BuildAndValidate_WithAnInvalidDelegate_Throws()
    {
        // arrange, act
        var exception = Record.Exception(() =>
            SecureHeadersBuilder.BuildAndValidate(opt => opt.UseCrossOriginEmbedderPolicy()));

        // assert
        Assert.NotNull(exception);
        var argEx = Assert.IsAssignableFrom<ArgumentException>(exception);
        Assert.Contains("Cross-Origin-Embedder-Policy", argEx.Message);
    }

    [Fact]
    public void BuildAndValidate_WithANullDelegate_Throws()
    {
        // arrange, act
        var exception = Record.Exception(() => SecureHeadersBuilder.BuildAndValidate(null));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentNullException>(exception);
    }

    [Fact]
    public void BuildAndValidate_OnAnEmptyDelegate_DoesNotThrow()
    {
        // arrange, act
        // An empty configuration is valid, if inert. The middleware warns about it at startup;
        // it is deliberately not an error, since it is a legitimate way to disable the
        // middleware without removing it from the pipeline.
        var exception = Record.Exception(() => SecureHeadersBuilder.BuildAndValidate(_ => { }));

        // assert
        Assert.Null(exception);
    }
}
