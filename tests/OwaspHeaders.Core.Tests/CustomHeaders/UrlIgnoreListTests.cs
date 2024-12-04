namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class UrlIgnoreListTests : SecureHeadersTests
{
    private readonly string UrlToIgnore = "/ignore-me";
    private readonly string UrlWontIgnore = "/do-not-ignore-me";

    [Fact]
    public async Task IgnoreList_Contains_TargetUrl_NoHeadersAdded()
    {
        // arrange
        var urlsToIgnore = new List<string> { UrlToIgnore };
        var config = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration(urlsToIgnore);
        TestServer = CreateTestServer(UrlWontIgnore, config);

        // Act
        var context = await TestServer.SendAsync(c =>
        {
            c.Request.Path = UrlToIgnore;
            c.Request.Method = HttpMethods.Get;
        });

        // Assert
        Assert.NotNull(context.Response);
        Assert.NotNull(context.Response.Headers);

        // Checking none of the default headers are present
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.StrictTransportSecurityHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XFrameOptionsHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XssProtectionHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.XContentTypeOptionsHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.ContentSecurityPolicyHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.PermittedCrossDomainPoliciesHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.ReferrerPolicyHeaderName);
        Assert.DoesNotContain(context.Response.Headers, h => h.Key == Constants.CacheControlHeaderName);
    }

    [Fact]
    public async Task IgnoreList_DoesntContain_TargetUrl_NoHeadersAdded()
    {
        // Arrange
        var urlsToIgnore = new List<string> { UrlToIgnore };
        var config = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration(urlsToIgnore);
        TestServer = CreateTestServer(UrlWontIgnore, config);

        // Act
        var context = await TestServer.SendAsync(c =>
        {
            c.Request.Path = UrlWontIgnore;
            c.Request.Method = HttpMethods.Get;
        });

        // Assert
        Assert.NotNull(context.Response);
        Assert.NotNull(context.Response.Headers);

        // Checking all of the default headers are present
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.StrictTransportSecurityHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XFrameOptionsHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XssProtectionHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.XContentTypeOptionsHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.ContentSecurityPolicyHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.PermittedCrossDomainPoliciesHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.ReferrerPolicyHeaderName);
        Assert.Contains(context.Response.Headers, h => h.Key == Constants.CacheControlHeaderName);
    }
}
