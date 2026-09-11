namespace OwaspHeaders.Core.Tests.CustomHeaders;

public abstract class SecureHeadersTests
{
    private int _onNextCalledTimes;
    private readonly Task _onNextResult = Task.FromResult(0);
    internal readonly RequestDelegate _onNext;
    internal readonly DefaultHttpContext _context;
    internal TestServer TestServer;

    protected SecureHeadersTests()
    {
        _onNext = _ =>
        {
            Interlocked.Increment(ref _onNextCalledTimes);
            return _onNextResult;
        };
        _context = new DefaultHttpContext();
    }

    [Fact]
    public void ConstructWith_NullConfig_ExceptionThrown()
    {
        // The configuration is checked when the middleware is constructed, which ASP.NET Core
        // does while building the request pipeline, so this fails at application start rather
        // than on the first request.
        var exception = Record.Exception(() => new SecureHeadersMiddleware(_onNext, null));

        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);

        var argEx = exception as ArgumentException;
        Assert.NotNull(argEx);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration), exception.Message);
    }

    internal TestServer CreateTestServer(string urlToMap, Action<SecureHeadersBuilder> configure = null,
        string urlToIgnore = null)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseSecureHeadersMiddleware(opt =>
                        {
                            if (configure == null)
                            {
                                opt.UseRecommendedDefaults();
                            }
                            else
                            {
                                configure(opt);
                            }

                            // Deliberately conditional. The ignore list is now honoured whatever
                            // else is configured, where the deprecated overload used to discard it
                            // whenever a configuration was supplied. Passing [null] through would
                            // put a null into UrlsToIgnore, which RequestShouldBeIgnored would
                            // then call Equals on.
                            if (!string.IsNullOrWhiteSpace(urlToIgnore))
                            {
                                opt.SetUrlsToIgnore([urlToIgnore]);
                            }
                        });
                        app.UseEndpoints(endpoints =>
                        {
                            if (!string.IsNullOrWhiteSpace(urlToIgnore))
                            {
                                endpoints.MapGet(urlToIgnore, () =>
                                    TypedResults.Text("Hello Tests; this will be ignored by the middleware"));
                            }
                            endpoints.MapGet(urlToMap, () =>
                                TypedResults.Text("Hello Tests"));
                        });
                    });
            })
            .Start();

        var testServer = host.GetTestServer();
        testServer.BaseAddress = new Uri("https://example.com/");

        return testServer;
    }
}
