namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// Regression tests for https://github.com/GaProgMan/OwaspHeaders.Core/issues/239
///
/// A <c>null</c> entry in <see cref="SecureHeadersMiddlewareConfiguration.UrlsToIgnore"/> used to
/// throw a <see cref="NullReferenceException"/> on every request which did not match an earlier
/// entry, because the middleware called <c>Equals</c> on each entry. In version 11,
/// <see cref="SecureHeadersBuilder.SetUrlsToIgnore"/> rejects a null entry when the configuration
/// is built, and the middleware still skips one which reaches the list some other way.
/// </summary>
public class Issue239RegressionTests
{
    private const string IgnoredUrl = "/skipthis";
    private const string OtherUrl = "/other";

    private readonly RequestDelegate _onNext = _ => Task.CompletedTask;

    [Fact]
    public void NullEntryFromBuilder_IsRejected()
    {
        // null! because passing a null entry is the point of the test
        var exception = Assert.Throws<ArgumentException>(() => new SecureHeadersBuilder()
            .UseHsts()
            .SetUrlsToIgnore([IgnoredUrl, null!]));

        Assert.Equal("urlsToIgnore", exception.ParamName);
    }

    [Fact]
    public async Task NullEntryFromConfigureDelegate_StopsTheHostFromStarting()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSecureHeadersMiddleware(opt =>
                        {
                            opt.UseHsts();
                            opt.SetUrlsToIgnore([null!, IgnoredUrl]);
                        });
                        app.Run(async context => await context.Response.WriteAsync("Hello Tests"));
                    });
            });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            hostBuilder.StartAsync(TestContext.Current.CancellationToken));
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
    public async Task NullEntryFromDirectAssignment_DoesNotStopALaterMatch()
    {
        var config = new SecureHeadersBuilder()
            .UseHsts()
            .Build();

        config.UrlsToIgnore = [null!, IgnoredUrl];

        var context = await Invoke(config, IgnoredUrl);

        Assert.False(context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task NullEntryFromUrlIgnoreList_StopsTheHostFromStarting()
    {
        // app.UseSecureHeadersMiddleware(urlIgnoreList: ...) with no configuration passes the
        // list through BuildDefaultConfiguration and so through SetUrlsToIgnore. That overload is
        // [Obsolete] in version 11 and removed in version 12 (#269), when this test goes with it
        var hostBuilder = new HostBuilder()
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
            });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            hostBuilder.StartAsync(TestContext.Current.CancellationToken));
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
