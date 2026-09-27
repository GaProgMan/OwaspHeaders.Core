namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class CacheControlHeaderOptionsTests
{
    private int _onNextCalledTimes;
    private readonly Task _onNextResult = Task.FromResult(0);
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public CacheControlHeaderOptionsTests()
    {
        _onNext = _ =>
        {
            Interlocked.Increment(ref _onNextCalledTimes);
            return _onNextResult;
        };
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task Invoke_CacheControl_IsPrivate_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseCacheControl(@private: true).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));

        _context.Response.Headers.TryGetValue(Constants.CacheControlHeaderName, out var headerValues);
        Assert.True(headerValues.Any());
        Assert.Equal("private, no-cache, no-store, max-age=0", headerValues.First());
    }

    [Fact]
    public async Task Invoke_CacheControl_MustRevalidate_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseCacheControl(mustRevalidate: true).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));

        _context.Response.Headers.TryGetValue(Constants.CacheControlHeaderName, out var headerValues);
        Assert.True(headerValues.Any());
        Assert.Equal("no-cache, no-store, max-age=0, must-revalidate", headerValues.First());
    }

    [Fact]
    public async Task Invoke_CacheControl_NoCache_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseCacheControl(noCache: true).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));

        _context.Response.Headers.TryGetValue(Constants.CacheControlHeaderName, out var headerValues);
        Assert.True(headerValues.Any());
        Assert.Equal("no-cache, no-store, max-age=0", headerValues.First());
    }

    [Fact]
    public async Task Invoke_CacheControl_NoStore_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseCacheControl(noStore: true).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));

        _context.Response.Headers.TryGetValue(Constants.CacheControlHeaderName, out var headerValues);
        Assert.True(headerValues.Any());
        Assert.Equal("no-cache, no-store, max-age=0", headerValues.First());
    }
}
