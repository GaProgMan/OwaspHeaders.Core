namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// Regression tests for https://github.com/GaProgMan/OwaspHeaders.Core/issues/239
///
/// A <c>null</c> entry in <see cref="SecureHeadersMiddlewareConfiguration.UrlsToIgnore"/> used to
/// throw a <see cref="NullReferenceException"/> on every request which did not match an earlier
/// entry, because the middleware called <c>Equals</c> on each entry. A null entry is now skipped,
/// whichever of the three routes put it there.
/// </summary>
public class Issue239RegressionTests
{
    private const string IgnoredUrl = "/skipthis";
    private const string OtherUrl = "/other";

    private readonly RequestDelegate _onNext = _ => Task.CompletedTask;

    [Fact]
    public async Task NullEntryFromBuilder_DoesNotThrow_AndHeadersAreAdded()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .SetUrlsToIgnore([IgnoredUrl, null])
            .Build();

        var context = await Invoke(config, OtherUrl);

        Assert.True(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task NullEntryFromDirectAssignment_DoesNotThrow_AndHeadersAreAdded()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .Build();

        // the public setter bypasses SetUrlsToIgnore entirely
        config.UrlsToIgnore = [null];

        var context = await Invoke(config, OtherUrl);

        Assert.True(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task NullEntryBeforeAMatch_DoesNotStopTheMatch()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .SetUrlsToIgnore([null, IgnoredUrl])
            .Build();

        var context = await Invoke(config, IgnoredUrl);

        Assert.False(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Theory]
    [InlineData(OtherUrl, true)]
    [InlineData(IgnoredUrl, false)]
    public async Task NullEntryFromUrlIgnoreList_DoesNotThrow(string path, bool expectHeaders)
    {
        // app.UseSecureHeadersMiddleware(urlIgnoreList: ...) with no configuration passes the
        // list through BuildDefaultConfiguration, so it reaches the middleware
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSecureHeadersMiddleware(urlIgnoreList: [null, IgnoredUrl]);
                        app.Run(async context => await context.Response.WriteAsync("Hello Tests"));
                    });
            })
            .StartAsync();

        var context = await host.GetTestServer().SendAsync(c =>
        {
            c.Request.Path = path;
            c.Request.Method = HttpMethods.Get;
        });

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal(expectHeaders,
            context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    private async Task<HttpContext> Invoke(SecureHeadersMiddlewareConfiguration config, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;

        var middleware = new SecureHeadersMiddleware(_onNext, config);
        await middleware.InvokeAsync(context);

        return context;
    }
}
