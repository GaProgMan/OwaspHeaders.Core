namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class CrossOriginOptionsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseCrossOriginResourcePolicyCalled_Header_Is_Present()
    {
        // arrange
        var headerPresentConfig =
            SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseCrossOriginResourcePolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCrossOriginResourcePolicy);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CrossOriginResourcePolicyHeaderName));
        Assert.Equal(CrossOriginResourcePolicy.SameOriginValue,
            _context.Response.Headers[Constants.CrossOriginResourcePolicyHeaderName]);
    }
    
    [Fact]
    public async Task When_UseCrossOriginOpenerPolicyCalled_Header_Is_Present()
    {
        // arrange
        var headerPresentConfig =
            SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseCrossOriginOpenerPolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCrossOriginOpenerPolicy);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CrossOriginOpenerPolicyHeaderName));
        Assert.Equal(CrossOriginOpenerPolicy.SameOriginValue,
            _context.Response.Headers[Constants.CrossOriginOpenerPolicyHeaderName]);
    }

    [Fact]
    public async Task When_UseCrossOriginResourcePolicyNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseCrossOriginResourcePolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.CrossOriginResourcePolicyHeaderName));
    }
    
    [Fact]
    public async Task When_UseCrossOriginOpenerPolicyNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseCrossOriginOpenerPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.CrossOriginOpenerPolicyHeaderName));
    }
}

