using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Tests.RegressionTests;

/// <summary>
/// Regression tests for https://github.com/GaProgMan/OwaspHeaders.Core/issues/240
///
/// Report-only Content-Security-Policy mode had three defects. Asking a report-only method for
/// X-Content-Security-Policy set the flag with no enforcing policy behind it. A report-only call
/// silently switched off an X-Content-Security-Policy enabled earlier. And SetCspUris and
/// SetCspSandBox were silently ignored in report-only mode, so the report-only header carried no
/// directives.
/// </summary>
public class Issue240RegressionTests
{
    private const string ReportUri = "https://example.com/csp-report";

    private readonly RequestDelegate _onNext = _ => Task.CompletedTask;

    [Fact]
    public void ReportUriOnly_WithXContentSecurityPolicy_Throws_ArgumentException_NamingTheParameter()
    {
        var exception = Record.Exception(() => new SecureHeadersBuilder()
            .UseContentSecurityPolicyReportUriOnly(ReportUri, useXContentSecurityPolicy: true));

        var argEx = Assert.IsType<ArgumentException>(exception);
        Assert.Equal("useXContentSecurityPolicy", argEx.ParamName);
        Assert.Contains("UseContentSecurityPolicy(useXContentSecurityPolicy: true)", argEx.Message);
    }

    [Fact]
    public void ReportUriOnly_WithXContentSecurityPolicy_LeavesTheConfigurationUntouched()
    {
        var builder = new SecureHeadersBuilder();

        _ = Record.Exception(() =>
            builder.UseContentSecurityPolicyReportUriOnly(ReportUri, useXContentSecurityPolicy: true));
        var config = builder.Build();

        Assert.False(config.UseContentSecurityPolicyReportOnly);
        Assert.False(config.UseXContentSecurityPolicy);
        Assert.Null(config.ContentSecurityPolicyReportOnlyConfiguration);
    }

    [Fact]
    public async Task ReportUriOnly_AfterEnforcingPolicyWithXContentSecurityPolicy_LeavesItEnabled()
    {
        var config = new SecureHeadersBuilder()
            .UseContentSecurityPolicy(useXContentSecurityPolicy: true)
            .UseContentSecurityPolicyReportUriOnly(ReportUri)
            .Build();
        var context = new DefaultHttpContext();

        await new SecureHeadersMiddleware(_onNext, config).InvokeAsync(context);

        Assert.True(config.UseXContentSecurityPolicy);
        Assert.Empty(config.Validate());
        Assert.Equal("block-all-mixed-content;upgrade-insecure-requests;",
            context.Response.Headers[Constants.XContentSecurityPolicyHeaderName]);
        Assert.Equal($"block-all-mixed-content;upgrade-insecure-requests;report-uri {ReportUri};",
            context.Response.Headers[Constants.ContentSecurityPolicyReportOnlyHeaderName]);
    }

    [Fact]
    public async Task SetCspUris_InReportOnlyMode_AppliesToTheReportOnlyPolicy()
    {
        var config = new SecureHeadersBuilder()
            .UseContentSecurityPolicyReportUriOnly(ReportUri, blockAllMixedContent: false,
                upgradeInsecureRequests: false)
            .SetCspUris([ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.DefaultUri)
            .Build();
        var context = new DefaultHttpContext();

        await new SecureHeadersMiddleware(_onNext, config).InvokeAsync(context);

        Assert.Equal($"default-src 'self';report-uri {ReportUri};",
            context.Response.Headers[Constants.ContentSecurityPolicyReportOnlyHeaderName]);
    }

    [Fact]
    public void SetCspUris_WithBothPolicies_GivesEachPolicyItsOwnCopy()
    {
        var config = new SecureHeadersBuilder()
            .UseContentSecurityPolicy()
            .UseContentSecurityPolicyReportUriOnly(ReportUri)
            .SetCspUris([ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.DefaultUri)
            .Build();

        var enforcing = config.ContentSecurityPolicyConfiguration.DefaultSrc;
        var reportOnly = config.ContentSecurityPolicyReportOnlyConfiguration.DefaultSrc;

        Assert.Single(enforcing);
        Assert.Single(reportOnly);
        Assert.NotSame(enforcing, reportOnly);
    }

    [Fact]
    public void UseDefaultContentSecurityPolicy_AfterReportOnly_DoesNotAddItsDirectivesToTheReportOnlyPolicy()
    {
        var config = new SecureHeadersBuilder()
            .UseContentSecurityPolicyReportUriOnly(ReportUri)
            .UseDefaultContentSecurityPolicy()
            .Build();

        Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;",
            config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        Assert.Equal($"block-all-mixed-content;upgrade-insecure-requests;report-uri {ReportUri};",
            config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue());
    }

    // The two tests below use deprecated API on purpose. The obsolete report-only alias and the
    // build-then-pass path both still ship in version 11, and the report-only example in the
    // documentation is written against the latter.
#pragma warning disable CS0618
    [Fact]
    public void ObsoleteReportOnlyAlias_WithXContentSecurityPolicy_Throws_ArgumentException()
    {
        var exception = Record.Exception(() => new SecureHeadersBuilder()
            .UseContentSecurityPolicyReportOnly(ReportUri, useXContentSecurityPolicy: true));

        var argEx = Assert.IsType<ArgumentException>(exception);
        Assert.Equal("useXContentSecurityPolicy", argEx.ParamName);
    }

    [Fact]
    public void DocumentedReportOnlyExample_ProducesTheDirectivesItDescribes()
    {
        var config = SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseContentSecurityPolicyReportOnly(
                reportUri: ReportUri,
                blockAllMixedContent: false,
                upgradeInsecureRequests: false)
            .SetCspUris([ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.DefaultUri)
            .Build();

        Assert.Equal($"default-src 'self';report-uri {ReportUri};",
            config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue());
    }
#pragma warning restore CS0618
}
