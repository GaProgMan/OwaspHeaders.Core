using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;
using Xunit;

namespace tests;

[ExcludeFromCodeCoverage]
public class SecureHeadersInjectedTest
{
    private int _onNextCalledTimes;
    private readonly Task _onNextResult = Task.FromResult(0);
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public SecureHeadersInjectedTest()
    {
        _onNext = _ =>
        {
            Interlocked.Increment(ref _onNextCalledTimes);
            return _onNextResult;
        };
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task Invoke_StrictTransportSecurityHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseHsts().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseHsts);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
        Assert.Equal("max-age=63072000;includeSubDomains",
            _context.Response.Headers[Constants.StrictTransportSecurityHeaderName]);
    }

    [Fact]
    public async Task Invoke_StrictTransportSecurityHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresetConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresetConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresetConfig.UseHsts);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
    }

    [Fact]
    public async Task Invoke_XFrameOptionsHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseXFrameOptions().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseXFrameOptions);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
        Assert.Equal("DENY", _context.Response.Headers[Constants.XFrameOptionsHeaderName]);
    }

    [Fact]
    public async Task Invoke_XFrameOptionsHeaderName_HeaderIsNotPresentInDefault()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXFrameOptions);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
    }

    [Fact]
    public async Task Invoke_XssProtectionHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseXssProtection().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseXssProtection);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
        Assert.Equal("0", _context.Response.Headers[Constants.XssProtectionHeaderName]);
    }

    [Fact]
    public async Task Invoke_XssProtectionHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXssProtection);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
    }


    [Fact]
    public async Task Invoke_XContentTypeOptionsHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentTypeOptions().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseXContentTypeOptions);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
        Assert.Equal("nosniff", _context.Response.Headers[Constants.XContentTypeOptionsHeaderName]);
    }

    [Fact]
    public async Task Invoke_XContentTypeOptionsHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXContentTypeOptions);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentDefaultSecurityPolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        if (headerPresentConfig.UseContentSecurityPolicy)
        {
            Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
            Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;",
                _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);
        }
        else
        {
            Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
        }
    }

    [Fact]
    public async Task invoke_NullConfig_ExceptionThrown()
    {
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, null);

        var exception = await Record.ExceptionAsync(() => secureHeadersMiddleware.InvokeAsync(_context));

        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);

        var argEx = exception as ArgumentException;
        Assert.NotNull(argEx);
        Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration), exception.Message);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsPresent_WithMultipleCspSandboxTypes()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentSecurityPolicy().Build();
        headerPresentConfig.SetCspSandBox(CspSandboxType.allowForms, CspSandboxType.allowScripts, CspSandboxType.allowSameOrigin);
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
        Assert.Equal("sandbox allow-forms allow-scripts allow-same-origin;block-all-mixed-content;upgrade-insecure-requests;",
            _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseContentSecurityPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyReportOnly_HeaderIsPresent_WithMultipleCspSandboxTypes()
    {
        const string reportUri = "https://localhost:5001/report-uri";
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentSecurityPolicyReportOnly(reportUri).Build();
        headerPresentConfig.SetCspSandBox(CspSandboxType.allowForms, CspSandboxType.allowScripts, CspSandboxType.allowSameOrigin);
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyReportOnlyHeaderName));
        Assert.Equal($"block-all-mixed-content;upgrade-insecure-requests;report-uri {reportUri};",
            _context.Response.Headers[Constants.ContentSecurityPolicyReportOnlyHeaderName]);
    }

    [Fact]
    public async Task Invoke_ContentSecurityPolicyReportOnly_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
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
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentSecurityPolicy(useXContentSecurityPolicy: true).Build();
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
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseXContentSecurityPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentSecurityPolicyHeaderName));
    }

    [Fact]
    public async Task Invoke_PermittedCrossDomainPoliciesHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig =
            SecureHeadersMiddlewareBuilder.CreateBuilder().UsePermittedCrossDomainPolicies().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UsePermittedCrossDomainPolicy);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
        Assert.Equal("none;",
            _context.Response.Headers[Constants.PermittedCrossDomainPoliciesHeaderName]);
    }

    [Fact]
    public async Task Invoke_PermittedCrossDomainPoliciesHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UsePermittedCrossDomainPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
    }

    [Fact]
    public async Task Invoke_ReferrerPolicyHeaderName_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseReferrerPolicy().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseReferrerPolicy);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
        Assert.Equal("no-referrer", _context.Response.Headers[Constants.ReferrerPolicyHeaderName]);
    }

    [Fact]
    public async Task Invoke_ReferrerPolicyHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseReferrerPolicy);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
    }

    [Fact]
    public async Task Invoke_ExpectCtHeaderName_HeaderIsPresent()
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
    public async Task Invoke_ExpectCtHeaderName_HeaderIsPresent_ReportUri_Optional()
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

    [Fact]
    public async Task Invoke_ExpectCtHeaderName_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseExpectCt);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
    }

    [Fact]
    public async Task Invoke_XPoweredByHeader_RemoveHeader()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().RemovePoweredByHeader().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.RemoveXPoweredByHeader);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.PoweredByHeaderName));
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ServerHeaderName));
    }

    [Fact]
    public async Task Invoke_XPoweredByHeader_DoNotRemoveHeader()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerPresentConfig.RemoveXPoweredByHeader);
        // Am currently running the 2.1.300 Preview 1 build of the SDK
        // and the server doesn't seem to add this header.
        // Therefore this assert is commented out, as it will always fail
        //Assert.True(_context.Response.Headers.ContainsKey(Constants.PoweredByHeaderName));
    }

    [Fact]
    public async Task Invoke_CacheControl_HeaderIsPresent()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseCacheControl().Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));
        Assert.Equal(headerPresentConfig.CacheControl.BuildHeaderValue(),
            _context.Response.Headers[Constants.CacheControlHeaderName]);
    }

    [Fact]
    public async Task Invoke_CacheControl_HeaderIsNotPresent()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder();
        headerNotPresentConfig.UseCacheControl = false;
        headerNotPresentConfig.Build();

        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseCacheControl);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.CacheControlHeaderName));
    }
}
