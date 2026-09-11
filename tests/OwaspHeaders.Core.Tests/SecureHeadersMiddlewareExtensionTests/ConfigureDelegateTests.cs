namespace OwaspHeaders.Core.Tests.SecureHeadersMiddlewareExtensionTests;

/// <summary>
/// Covers the configure-delegate overload of <c>UseSecureHeadersMiddleware</c>, end to end
/// through a real request pipeline.
/// </summary>
public class ConfigureDelegateTests
{
    [Fact]
    public async Task ConfigureDelegate_AppliesTheConfiguration()
    {
        // arrange
        using var server = CreateTestServer(opt =>
        {
            opt.UseHsts();
            opt.UseXFrameOptions();
        });

        // act
        var response = await server.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);

        // assert
        Assert.True(response.Headers.Contains(Constants.StrictTransportSecurityHeaderName));
        Assert.True(response.Headers.Contains(Constants.XFrameOptionsHeaderName));
        Assert.False(response.Headers.Contains(Constants.ReferrerPolicyHeaderName));
    }

    [Fact]
    public async Task ConfigureDelegate_WithRecommendedDefaults_MatchesTheParameterlessOverload()
    {
        // arrange
        using var viaDelegate = CreateTestServer(opt => opt.UseRecommendedDefaults());
        using var viaOneLiner = CreateTestServer(configure: null);

        // act
        var delegateResponse = await viaDelegate.CreateClient()
            .GetAsync("/", TestContext.Current.CancellationToken);
        var oneLinerResponse = await viaOneLiner.CreateClient()
            .GetAsync("/", TestContext.Current.CancellationToken);

        // assert
        var delegateHeaders = delegateResponse.Headers.Select(h => h.Key).OrderBy(h => h).ToList();
        var oneLinerHeaders = oneLinerResponse.Headers.Select(h => h.Key).OrderBy(h => h).ToList();
        Assert.Equal(oneLinerHeaders, delegateHeaders);
        Assert.NotEmpty(delegateHeaders);
    }

    [Fact]
    public async Task ConfigureDelegate_HonoursSetUrlsToIgnore()
    {
        // arrange
        using var server = CreateTestServer(opt =>
        {
            opt.UseRecommendedDefaults();
            opt.SetUrlsToIgnore(["/skipthis"]);
        });

        // act
        var response = await server.CreateClient()
            .GetAsync("/skipthis", TestContext.Current.CancellationToken);

        // assert
        Assert.False(response.Headers.Contains(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public void ConfigureDelegate_ThatProducesAnInvalidConfiguration_ThrowsWhenTheHostStarts()
    {
        // arrange, act
        // Cross-Origin-Embedder-Policy without Cross-Origin-Resource-Policy. Before validation
        // moved to startup this would have started cleanly and thrown on the first request.
        var exception = Record.Exception(() =>
            CreateTestServer(opt => opt.UseCrossOriginEmbedderPolicy()));

        // assert
        Assert.NotNull(exception);
        var argEx = Assert.IsAssignableFrom<ArgumentException>(exception);
        Assert.Contains("Cross-Origin-Embedder-Policy", argEx.Message);
    }

    [Fact]
    public void ConfigureDelegate_WithANullDelegate_Throws()
    {
        // arrange
        var applicationBuilder = new SecureHeadersMiddlewareTests.MockedApplicationBuilder();

        // act
        var exception = Record.Exception(() =>
            applicationBuilder.UseSecureHeadersMiddleware((Action<SecureHeadersBuilder>)null));

        // assert
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ConfigureDelegate_IsInvokedExactlyOnce()
    {
        // arrange
        var invocations = 0;

        // act
        using var server = CreateTestServer(_ => invocations++);

        // assert
        // The delegate runs while the pipeline is built, not per request.
        Assert.Equal(1, invocations);
    }

    private static TestServer CreateTestServer(Action<SecureHeadersBuilder> configure)
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

                        if (configure == null)
                        {
                            app.UseSecureHeadersMiddleware();
                        }
                        else
                        {
                            app.UseSecureHeadersMiddleware(configure);
                        }

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", () => TypedResults.Text("Hello Tests"));
                            endpoints.MapGet("/skipthis", () => TypedResults.Text("Ignored"));
                        });
                    });
            })
            .Start();

        return host.GetTestServer();
    }
}
