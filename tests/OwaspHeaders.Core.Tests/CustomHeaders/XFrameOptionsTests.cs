namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class XFrameOptionsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseXFrameOptionsNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXFrameOptions);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
    }
}
