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
        // null! because passing a null entry is the point of the test
        var config = new SecureHeadersBuilder()
            .UseHsts()
            .SetUrlsToIgnore([IgnoredUrl, null!])
            .Build();

        var context = await Invoke(config, OtherUrl);

        Assert.True(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task NullEntryFromDirectAssignment_DoesNotThrow_AndHeadersAreAdded()
    {
        var config = new SecureHeadersBuilder()
            .UseHsts()
            .Build();

        // the setter is internal in version 11, so only code inside the assembly can do this.
        // It is kept as the test of the middleware's null-safe comparison, which is the backstop
        // for a null which reaches the list without going through SetUrlsToIgnore
        config.UrlsToIgnore = [null!];

        var context = await Invoke(config, OtherUrl);

        Assert.True(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task NullEntryBeforeAMatch_DoesNotStopTheMatch()
    {
        var config = new SecureHeadersBuilder()
            .UseHsts()
            .SetUrlsToIgnore([null!, IgnoredUrl])
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
        // list through BuildDefaultConfiguration, so it reaches the middleware. That overload is
        // [Obsolete] in version 11 and removed in version 12 (#269), when this test goes with it
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
#pragma warning disable CS0618
                        app.UseSecureHeadersMiddleware(urlIgnoreList: [null!, IgnoredUrl]);
#pragma warning restore CS0618
                        app.Run(async context => await context.Response.WriteAsync("Hello Tests"));
                    });
            })
            .StartAsync(TestContext.Current.CancellationToken);

        var context = await host.GetTestServer().SendAsync(c =>
        {
            c.Request.Path = path;
            c.Request.Method = HttpMethods.Get;
        }, TestContext.Current.CancellationToken);

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
