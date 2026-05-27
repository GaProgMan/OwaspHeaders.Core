namespace OwaspHeaders.Core.Tests.CustomHeaders;

// Expect-CT has been deprecated by OWASP and UseExpectCt is marked [Obsolete].
// These tests still exercise the opt-in code path while it exists in the codebase.
#pragma warning disable CS0618
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
    public void Default_Configuration_Does_Not_Include_ExpectCt()
    {
        // arrange / act
        var defaultConfig = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration();

        // assert
        Assert.False(defaultConfig.UseExpectCt,
            "Expect-CT has been deprecated by OWASP and must not be enabled by BuildDefaultConfiguration.");
        Assert.Null(defaultConfig.ExpectCt);
    }

    [Fact]
    public void UseExpectCt_Is_Marked_Obsolete()
    {
        // arrange
        var method = typeof(SecureHeadersMiddlewareBuilder)
            .GetMethod(nameof(SecureHeadersMiddlewareBuilder.UseExpectCt));

        // act
        var obsolete = method?.GetCustomAttribute<ObsoleteAttribute>();

        // assert
        Assert.NotNull(obsolete);
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
#pragma warning restore CS0618
