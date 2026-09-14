namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class DefaultSecureHeadersIntegrationTests : SecureHeadersTests
{
    [Fact]
    public async Task AllHeaders_Present_When_BuildDefault_Used()
    {
        // arrange
        const string testUrl = "/hello";

        // One delegate, used for both the pipeline and the assertions below. This is the shape
        // consumers are meant to adopt: hoist the configuration into a named method, hand it to
        // UseSecureHeadersMiddleware, and assert on it from a test via BuildAndValidate.
        static void Configure(SecureHeadersBuilder opt)
        {
            opt.UseRecommendedDefaults();
            opt.UseDefaultContentSecurityPolicy();
        }

        var headerPresentConfig = SecureHeadersBuilder.BuildAndValidate(Configure);
        TestServer = CreateTestServer(testUrl, Configure);

        // act
        var context = await TestServer.SendAsync(c =>
        {
            c.Request.Path = testUrl;
            c.Request.Method = HttpMethods.Get;
        }, TestContext.Current.CancellationToken);

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
