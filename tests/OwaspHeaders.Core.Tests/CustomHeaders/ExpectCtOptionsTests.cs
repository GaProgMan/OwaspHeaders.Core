namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ExpectCtOptionsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseExpectCtCalled_Header_Is_Present()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseExpectCt("https://test.com/report").Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseExpectCt);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
        Assert.Equal(headerPresentConfig.ExpectCt.BuildHeaderValue(),
            _context.Response.Headers[Constants.ExpectCtHeaderName]);
    }

    [Fact]
    public async Task When_UseExpectCtNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseExpectCt);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
    }

    [Fact]
    public async Task When_UseExpectCtCalled_HeaderIsPresent_ReportUri_Optional()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseExpectCt(string.Empty).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseExpectCt);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
        Assert.Equal(headerPresentConfig.ExpectCt.BuildHeaderValue(),
            _context.Response.Headers[Constants.ExpectCtHeaderName]);
    }
}
