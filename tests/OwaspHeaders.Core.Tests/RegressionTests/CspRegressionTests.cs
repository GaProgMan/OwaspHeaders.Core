namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// This class contains a number of regression tests against bugs which were reported
/// using GitHub issues. Each test will link to the issue in question. Please make sure
/// that these tests still pass whenever making changes to this codebase
/// </summary>
public class CspRegressionTests
{
    private int _onNextCalledTimes;
    private readonly Task _onNextResult = Task.FromResult(0);
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public CspRegressionTests()
    {
        _onNext = _ =>
        {
            Interlocked.Increment(ref _onNextCalledTimes);
            return _onNextResult;
        };
        _context = new DefaultHttpContext();
    }

    /// <summary>
    /// This test exercises and provides regression against https://github.com/GaProgMan/OwaspHeaders.Core/issues/61
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_Adds_MultipleSameValue()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentSecurityPolicy()
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new()
                    {
                        CommandType = CspCommandType.Directive, DirectiveOrUri = "self"
                    },
                    new()
                    {
                        CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com"
                    }
                }, CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerValue = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.Equal(1,
            headerValue.First()
                .Split(" ")
                .Count(hv => hv.Contains("cdnjs.cloudflare.com", StringComparison.InvariantCultureIgnoreCase)));
    }

    /// <summary>
    /// This test exercises and proves regression against unnecessary spaces being added between the final
    /// directive and first URI in a CSP
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_Adds_UnnecessarySpace_Between_FinalDirective_And_First_Uri()
    {
        // arrange
        const string targetCsp = "style-src 'self' cdnjs.cloudflare.com;";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
                // originally PRODUCES: style-src 'self'  cdnjs.cloudflare.com;
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com" }
                }, CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        var actualCharCount = actualCsp.Length;
        var targetCharCount = targetCsp.Length;

        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));
        Assert.Equal(targetCharCount, actualCharCount);
    }

    /// <summary>
    /// This test exercises and proves regression against unnecessary spaces being added after a
    /// directive in a generated CSP, when no URIs are provided
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_Adds_UnnecessarySpace_After_FinalDirective_When_OnlyDirectivesProvided()
    {
        // arrange
        const string targetCsp = "style-src 'self';";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
                // originally PRODUCES: style-src 'self' ;
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" }
                }, CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        var actualCharCount = actualCsp.Length;
        var targetCharCount = targetCsp.Length;

        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));
        Assert.Equal(targetCharCount, actualCharCount);
    }

    /// <summary>
    /// This test exercises and proves regression against unnecessary spaces being added before the first
    /// URI in a CSP, when no Directives are provided
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_Adds_UnnecessarySpace_Before_First_Uri_When_NoDirectivesProvided()
    {
        // arrange
        const string targetCsp = "style-src cdnjs.cloudflare.com;";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
                // originally PRODUCES: style-src  cdnjs.cloudflare.com;
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com" }
                }, CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        var actualCharCount = actualCsp.Length;
        var targetCharCount = targetCsp.Length;

        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));
        Assert.Equal(targetCharCount, actualCharCount);
    }
}