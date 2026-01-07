namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ContentSecurityPolicyOptionsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseContentDefaultSecurityPolicyNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseContentSecurityPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsPresent_WithMultipleCspSandboxTypes()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy().Build();
        headerPresentConfig.SetCspSandBox(CspSandboxType.allowForms, CspSandboxType.allowScripts,
            CspSandboxType.allowSameOrigin);
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
        Assert.Equal(
            "sandbox allow-forms allow-scripts allow-same-origin;block-all-mixed-content;upgrade-insecure-requests;",
            _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyReportOnly_HeaderIsPresent_WithMultipleCspSandboxTypes()
    {
        const string reportUri = "https://localhost:5001/report-uri";
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicyReportUriOnly(reportUri).Build();
        headerPresentConfig.SetCspSandBox(CspSandboxType.allowForms, CspSandboxType.allowScripts,
            CspSandboxType.allowSameOrigin);
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyReportOnlyHeaderName));
        Assert.Equal($"block-all-mixed-content;upgrade-insecure-requests;report-uri {reportUri};",
            _context.Response.Headers[Constants.ContentSecurityPolicyReportOnlyHeaderName]);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyReportToOnly_HeaderIsPresent_WithMultipleCspSandboxTypes()
    {
        const string reportTo = "report-endpoint";
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(reportTo: reportTo).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
        var result = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToString();
        Assert.Contains($"report-to {reportTo}", result);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyReportOnly_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseContentSecurityPolicyReportOnly);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyReportOnlyHeaderName));
    }

    [Fact]
    public async Task Invoke_XContentSecurityPolicyHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(useXContentSecurityPolicy: true).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseXContentSecurityPolicy);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.XContentSecurityPolicyHeaderName));
        Assert.Equal("block-all-mixed-content;upgrade-insecure-requests;",
            _context.Response.Headers[Constants.XContentSecurityPolicyHeaderName]);

    }

    [Fact]
    public async Task Invoke_XContentSecurityPolicyHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXContentSecurityPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentSecurityPolicyHeaderName));
    }
}
