namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class DefaultSecureHeadersIntegrationTests : SecureHeadersTests
{
    [Fact]
    public async Task AllHeaders_Present_When_BuildDefault_Used()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareExtensions
            .BuildDefaultConfiguration()
            .UseDefaultContentSecurityPolicy();
        const string testUrl = "/hello";
        TestServer = CreateTestServer(testUrl, headerPresentConfig);

        // act
        var context = await TestServer.SendAsync(c =>
        {
            c.Request.Path = testUrl;
            c.Request.Method = HttpMethods.Get;
        });

        // assert
        Assert.True(headerPresentConfig.UseHsts);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.StrictTransportSecurityHeaderName);
        Assert.Equal("max-age=31536000;includeSubDomains",
            context.Response.Headers[Constants.StrictTransportSecurityHeaderName]);

        Assert.True(headerPresentConfig.UseXFrameOptions);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XFrameOptionsHeaderName);
        Assert.Equal("deny", context.Response.Headers[Constants.XFrameOptionsHeaderName]);

        Assert.True(headerPresentConfig.UseXssProtection);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XssProtectionHeaderName);
        Assert.Equal("0", context.Response.Headers[Constants.XssProtectionHeaderName]);

        Assert.True(headerPresentConfig.UseXContentTypeOptions);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XContentTypeOptionsHeaderName);
        Assert.Equal("nosniff", context.Response.Headers[Constants.XContentTypeOptionsHeaderName]);

        Assert.True(headerPresentConfig.UseContentSecurityPolicy);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.ContentSecurityPolicyHeaderName);
        Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;",
            context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);

        Assert.True(headerPresentConfig.UsePermittedCrossDomainPolicy);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.PermittedCrossDomainPoliciesHeaderName);
        Assert.Equal("none", context.Response.Headers[Constants.PermittedCrossDomainPoliciesHeaderName]);

        Assert.True(headerPresentConfig.UseReferrerPolicy);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.ReferrerPolicyHeaderName);
        Assert.Equal("no-referrer", context.Response.Headers[Constants.ReferrerPolicyHeaderName]);

        Assert.True(headerPresentConfig.UseCacheControl);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.CacheControlHeaderName);
        Assert.Equal("max-age=0,no-store", context.Response.Headers[Constants.CacheControlHeaderName]);
    }
}
