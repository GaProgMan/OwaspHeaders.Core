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
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com" }
            ], CspUriType.Style).Build();
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
                [
                    new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com" }
                ], CspUriType.Style).Build();
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
                [new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" }], CspUriType.Style).Build();
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
                [new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com" }], CspUriType.Style).Build();
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
    /// This test verifies that CspCommandType.Directive values are properly quoted in the final CSP header.
    /// Directive values like 'self', 'unsafe-inline', 'none' should be enclosed in single quotes.
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_DirectiveType_AddsQuotes_AroundDirectiveValues()
    {
        // arrange
        const string targetCsp = "script-src 'self' 'unsafe-inline' 'none';";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "none" }
            ], CspUriType.Script).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));

        // Verify individual directive values are quoted
        Assert.Contains("'self'", actualCsp);
        Assert.Contains("'unsafe-inline'", actualCsp);
        Assert.Contains("'none'", actualCsp);
    }

    /// <summary>
    /// This test verifies that CspCommandType.Uri values are NOT quoted in the final CSP header.
    /// URI values like URLs, domains, and URI schemes should appear without quotes.
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_UriType_DoesNotAddQuotes_AroundUriValues()
    {
        // arrange
        const string targetCsp = "img-src https://example.com *.googleapis.com data: blob:;";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://example.com" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "*.googleapis.com" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "blob:" }
            ], CspUriType.Img).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));

        // Verify URI values are NOT quoted
        Assert.Contains("https://example.com", actualCsp);
        Assert.Contains("*.googleapis.com", actualCsp);
        Assert.Contains("data:", actualCsp);
        Assert.Contains("blob:", actualCsp);

        // Ensure no quotes around URI values
        Assert.DoesNotContain("'https://example.com'", actualCsp);
        Assert.DoesNotContain("'*.googleapis.com'", actualCsp);
        Assert.DoesNotContain("'data:'", actualCsp);
        Assert.DoesNotContain("'blob:'", actualCsp);
    }

    /// <summary>
    /// This test specifically verifies the data: URI scheme configuration for inline SVG images,
    /// addressing the common user confusion about using data: with CspCommandType.Uri.
    /// This test ensures the reported bug scenario works correctly.
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_DataUriScheme_ForInlineSvgImages_FormatsCorrectly()
    {
        // arrange - This mirrors the user's intended CSP directive:
        // img-src 'self' data: https://cdn.abc.net https://cdn.abc.org;
        const string targetCsp = "img-src 'self' data: https://cdn.abc.net https://cdn.abc.org;";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" }, // This was the user's issue
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdn.abc.net" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdn.abc.org" }
            ], CspUriType.Img).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));

        // Verify specific formatting expectations
        Assert.Contains("'self'", actualCsp); // Directive should be quoted
        Assert.Contains("data:", actualCsp); // URI scheme should NOT be quoted
        Assert.DoesNotContain("'data:'", actualCsp); // Ensure data: is not incorrectly quoted
        Assert.Contains("https://cdn.abc.net", actualCsp);
        Assert.Contains("https://cdn.abc.org", actualCsp);
    }

    /// <summary>
    /// This test verifies mixed directive and URI elements format correctly when used together.
    /// It ensures proper spacing and quoting behavior between different CommandType values.
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_MixedDirectiveAndUri_FormatsCorrectlyWithProperSpacing()
    {
        // arrange
        const string targetCsp = "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com data:;";
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseContentSecurityPolicy(blockAllMixedContent: false, upgradeInsecureRequests: false)
            .SetCspUris(
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://fonts.googleapis.com" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdnjs.cloudflare.com" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" }
            ], CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerStrings = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.NotNull(headerStrings);

        var actualCsp = headerStrings.First();
        Assert.True(string.Equals(targetCsp, actualCsp, StringComparison.InvariantCultureIgnoreCase));

        // Verify proper spacing - no double spaces
        Assert.DoesNotContain("  ", actualCsp); // No double spaces

        // Verify correct quoting
        Assert.Contains("'self'", actualCsp);
        Assert.Contains("'unsafe-inline'", actualCsp);
        Assert.DoesNotContain("'https://fonts.googleapis.com'", actualCsp);
        Assert.DoesNotContain("'https://cdnjs.cloudflare.com'", actualCsp);
        Assert.DoesNotContain("'data:'", actualCsp);
    }
}
