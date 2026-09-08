// The build-then-pass configuration path is deprecated in version 11 and will be removed in
// version 12. It still ships, so it still needs to work, not merely compile. Everything in this
// file deliberately exercises the deprecated surface; the suppression below is the point of the
// file rather than a way around the warning, and the whole file goes away with the API in v12.
#pragma warning disable CS0618

namespace OwaspHeaders.Core.Tests.RegressionTests;

public class DeprecatedBuilderPathTests
{
    private readonly RequestDelegate _onNext = _ => Task.CompletedTask;

    [Fact]
    public void CreateBuilder_ReturnsAnEmptyConfiguration()
    {
        // arrange, act
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder();

        // assert
        Assert.NotNull(config);
        Assert.False(config.UseHsts);
    }

    [Fact]
    public void DeprecatedExtensions_MutateTheSameInstanceTheyAreGiven()
    {
        // arrange
        // The forwarders wrap the caller's configuration rather than allocating a new one, so
        // the fluent chain must keep handing back the very same object it was given. If that
        // ever stopped being true, existing consumer code would silently lose configuration.
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder();

        // act
        var returned = config.UseHsts().UseXFrameOptions().Build();

        // assert
        Assert.Same(config, returned);
        Assert.True(config.UseHsts);
        Assert.True(config.UseXFrameOptions);
    }

    [Fact]
    public void DeprecatedChain_ProducesTheSameHeadersAsTheBuilder()
    {
        // arrange
        var viaDeprecated = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseCacheControl()
            .Build();

        var viaBuilder = new SecureHeadersBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseCacheControl()
            .Build();

        // act
        var deprecatedHeaders = HeaderNamesFor(viaDeprecated);
        var builderHeaders = HeaderNamesFor(viaBuilder);

        // assert
        Assert.Equal(builderHeaders, deprecatedHeaders);
    }

    [Fact]
    public void DeprecatedChain_AddClearSiteDataPath_StillAccumulates()
    {
        // arrange, act
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.cookies)
            .AddClearSiteDataPath("/signout", ClearSiteDataOptions.cache)
            .Build();

        // assert
        Assert.Equal(2, config.ClearSiteDataPathConfiguration.PathConfigurations.Count);
    }

    [Fact]
    public void DeprecatedChain_SetCspUris_StillApplies()
    {
        // arrange, act
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseContentSecurityPolicy()
            .SetCspUris(
                [new ContentSecurityPolicyElement
                {
                    CommandType = CspCommandType.Directive, DirectiveOrUri = "self"
                }],
                CspUriType.Script)
            .Build();

        // assert
        Assert.Contains("script-src 'self'", config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
    }

    [Fact]
    public async Task DeprecatedOverload_AppliesTheSuppliedConfiguration()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        // act
        using var server = CreateTestServer(app => app.UseSecureHeadersMiddleware(config));
        var response = await server.CreateClient()
            .GetAsync("/", TestContext.Current.CancellationToken);

        // assert
        Assert.True(response.Headers.Contains(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public void DeprecatedOverload_WithAnInvalidConfiguration_ThrowsAtStartup()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        config.UseHsts = true;

        // act
        // Startup validation applies to the deprecated path too: deprecated does not mean
        // unvalidated for as long as the overload still ships.
        var exception = Record.Exception(() =>
            CreateTestServer(app => app.UseSecureHeadersMiddleware(config)));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);
    }

    [Fact]
    public async Task DeprecatedOverload_StillIgnoresUrlIgnoreListWhenAConfigIsSupplied()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder().UseHsts().Build();

        // act
        // Documenting the quirk rather than endorsing it: this overload drops urlIgnoreList
        // whenever config is supplied. It is one of the reasons the overload is going away, and
        // the replacement (opt.SetUrlsToIgnore) always applies.
        using var server = CreateTestServer(app =>
            app.UseSecureHeadersMiddleware(config, urlIgnoreList: ["/"]));
        var response = await server.CreateClient()
            .GetAsync("/", TestContext.Current.CancellationToken);

        // assert
        Assert.True(response.Headers.Contains(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public void DeprecatedBuildDefaultConfiguration_MatchesTheParameterlessOverload()
    {
        // arrange
        var viaDeprecated = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration();
        var viaPreset = new SecureHeadersBuilder().UseRecommendedDefaults().Build();

        // act, assert
        Assert.Equal(HeaderNamesFor(viaPreset), HeaderNamesFor(viaDeprecated));
    }

    private static List<string> HeaderNamesFor(SecureHeadersMiddlewareConfiguration config)
    {
        var middleware = new SecureHeadersMiddleware(_ => Task.CompletedTask, config);
        var context = new DefaultHttpContext();
        middleware.InvokeAsync(context).GetAwaiter().GetResult();

        return context.Response.Headers.Select(h => h.Key).OrderBy(h => h).ToList();
    }

    private static TestServer CreateTestServer(Action<IApplicationBuilder> configureApp)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services => services.AddRouting())
                    .Configure(app =>
                    {
                        app.UseRouting();
                        configureApp(app);
                        app.UseEndpoints(endpoints =>
                            endpoints.MapGet("/", () => TypedResults.Text("Hello Tests")));
                    });
            })
            .Start();

        return host.GetTestServer();
    }
}

#pragma warning restore CS0618
