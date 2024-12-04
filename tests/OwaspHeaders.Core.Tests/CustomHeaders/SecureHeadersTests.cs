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
    public async Task InvokeWith_NullConfig_ExceptionThrown()
    {
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, null);

        var exception = await Record.ExceptionAsync(() => secureHeadersMiddleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);

        var argEx = exception as ArgumentException;
        Assert.NotNull(argEx);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration), exception.Message);
    }

    internal TestServer CreateTestServer(string urlToMap, SecureHeadersMiddlewareConfiguration config = null,
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
                        app.UseSecureHeadersMiddleware(config, urlIgnoreList: [urlToIgnore]);
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
