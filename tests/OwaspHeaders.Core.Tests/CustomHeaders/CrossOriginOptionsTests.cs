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
        Assert.Equal("same-origin", _context.Response.Headers[Constants.CrossOriginResourcePolicyHeaderName]);
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
        Assert.Equal("same-origin", _context.Response.Headers[Constants.CrossOriginOpenerPolicyHeaderName]);
    }

    [Fact]
    public async Task When_UseCrossOriginEmbedderPolicyCalled_Header_Is_Present()
    {
        // arrange
        var headerPresentConfig =
            SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseCrossOriginResourcePolicy()
                .UseCrossOriginEmbedderPolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCrossOriginEmbedderPolicy);
        Assert.True(headerPresentConfig.UseCrossOriginResourcePolicy);

        Assert.True(_context.Response.Headers.ContainsKey(Constants.CrossOriginResourcePolicyHeaderName));
        Assert.Equal("same-origin", _context.Response.Headers[Constants.CrossOriginResourcePolicyHeaderName]);

        Assert.True(_context.Response.Headers.ContainsKey(Constants.CrossOriginEmbedderPolicyHeaderName));
        Assert.Equal("require-corp", _context.Response.Headers[Constants.CrossOriginEmbedderPolicyHeaderName]);
    }

    [Fact]
    public async Task When_UseCrossOriginEmbedderPolicyCalled_But_UseCrossOriginResourcePolicy_NotSupplied_Header_Is_Not_Present()
    {
        // arrange
        var headerPresentConfig =
            SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseCrossOriginEmbedderPolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        var exception = await Record.ExceptionAsync(() => secureHeadersMiddleware.InvokeAsync(_context));

        // assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);

        Assert.True(headerPresentConfig.UseCrossOriginEmbedderPolicy);
        Assert.False(headerPresentConfig.UseCrossOriginResourcePolicy);

        Assert.False(_context.Response.Headers.ContainsKey(Constants.CrossOriginEmbedderPolicyHeaderName));
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

    [Fact]
    public async Task When_UseCrossOriginEmbedderPolicyNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseCrossOriginEmbedderPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.CrossOriginEmbedderPolicyHeaderName));
    }

    [Theory]
    [InlineData(CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.RequireCorp)]
    [InlineData(CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.UnsafeNone)]
    public void CrossOriginEmbedderPolicy_HeaderValueIsValid_Returns_True_When_HeaderIsValid(CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions headerValue)
    {
        // Arrange
        var header = new CrossOriginEmbedderPolicy(headerValue);
        const bool useCorp = true;

        // Act
        var valid = header.HeaderValueIsValid(useCorp);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void CrossOriginEmbedderPolicy_HeaderValueIsValid_Returns_False_When_HeaderIsInvalid()
    {
        // Arrange
        var header = new CrossOriginEmbedderPolicy(CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.RequireCorp);
        var useCorp = false;

        // Act
        var valid = header.HeaderValueIsValid(useCorp);

        // Assert
        Assert.False(valid);
    }

}

