using Microsoft.AspNetCore.Http.Features;

namespace OwaspHeaders.Core.Tests.SecureHeadersMiddlewareExtensionTests;

public class SecureHeadersMiddlewareTests
{
    internal class MockedApplicationBuilder : IApplicationBuilder
    {
        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder New()
        {
            throw new NotImplementedException();
        }

        public RequestDelegate Build()
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ApplicationServices { get; set; }
        public IFeatureCollection ServerFeatures { get; }
        public IDictionary<string, object> Properties { get; }
    }

    [Fact]
    public void Raises_ArgumentNullException_If_IApplicationBuilder_IsNull()
    {
        // Arrange
        MockedApplicationBuilder mockedApplicationBuilder = null;

        // Act
        var result = Record.Exception(() => mockedApplicationBuilder.UseSecureHeadersMiddleware());

        // Assert
        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void BuildDefaultConfiguration_Returns_Valid_Configuration()
    {
        // Arrange

        // Act
        var middlewareConfiguration = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration();

        // Assert
        Assert.NotNull(middlewareConfiguration);
        Assert.IsType<SecureHeadersMiddlewareConfiguration>(middlewareConfiguration);

        AssertHeadersInResponse(middlewareConfiguration);
    }

    [Fact]
    public void BuildDefaultConfiguration_WhenValidIgnoreListSupplied_Returns_Valid_Configuration()
    {
        // Arrange
        var ignoreList = new List<string> { "/ignore" };

        // Act
        var middlewareConfiguration = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration(ignoreList);

        // Assert
        Assert.NotNull(middlewareConfiguration);
        Assert.IsType<SecureHeadersMiddlewareConfiguration>(middlewareConfiguration);

        AssertHeadersInResponse(middlewareConfiguration);

        // Ignore List
        Assert.NotNull(middlewareConfiguration.UrlsToIgnore);
        Assert.NotEmpty(middlewareConfiguration.UrlsToIgnore);
        Assert.Contains(ignoreList.First(), middlewareConfiguration.UrlsToIgnore);
    }

    [Fact]
    public void BuildDefaultConfiguration_WhenInvalidIgnoreListSupplied_Returns_Valid_Configuration_With_Empty_IgnoreList()
    {
        // Arrange
        List<string> ignoreList = null;

        // Act
        var middlewareConfiguration = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration(ignoreList);

        // Assert
        Assert.NotNull(middlewareConfiguration);
        Assert.IsType<SecureHeadersMiddlewareConfiguration>(middlewareConfiguration);

        AssertHeadersInResponse(middlewareConfiguration);

        // Ignore List
        Assert.NotNull(middlewareConfiguration.UrlsToIgnore);
        Assert.Empty(middlewareConfiguration.UrlsToIgnore);
    }

    private void AssertHeadersInResponse(SecureHeadersMiddlewareConfiguration middlewareConfiguration)
    {
        // HSTS
        Assert.True(middlewareConfiguration.UseHsts);
        Assert.Equal("max-age=31536000;includeSubDomains", middlewareConfiguration.HstsConfiguration.BuildHeaderValue());

        // X-Frame-Options
        Assert.True(middlewareConfiguration.UseXFrameOptions);
        Assert.Equal("DENY", middlewareConfiguration.XFrameOptionsConfiguration.BuildHeaderValue());

        // X-Content-Type-Options
        Assert.True(middlewareConfiguration.UseXContentTypeOptions);
        // Can't easily assert the value here, as it's set in the InvokeAsync for the middleware

        // Content-Security-Policy
        Assert.True(middlewareConfiguration.UseContentSecurityPolicy);
        Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;",
            middlewareConfiguration.ContentSecurityPolicyConfiguration.BuildHeaderValue());

        // X-Permitted-Cross-Domain-Policies
        Assert.True(middlewareConfiguration.UsePermittedCrossDomainPolicy);
        Assert.Equal("none;", middlewareConfiguration.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());

        // Referrer-Policy
        Assert.True(middlewareConfiguration.UseReferrerPolicy);
        Assert.Equal("no-referrer", middlewareConfiguration.ReferrerPolicy.BuildHeaderValue());

        // Cache-Control
        Assert.True(middlewareConfiguration.UseCacheControl);
        Assert.Equal("max-age=0,no-store", middlewareConfiguration.CacheControl.BuildHeaderValue());

        // X-XSS-Protection
        Assert.True(middlewareConfiguration.UseXssProtection);
        Assert.Equal("0", middlewareConfiguration.XssConfiguration.BuildHeaderValue());

        // Cross-Origin Resource Policy
        Assert.True(middlewareConfiguration.UseCrossOriginResourcePolicy);
        Assert.Equal("same-origin", middlewareConfiguration.CrossOriginResourcePolicy.BuildHeaderValue());
        
        // Cross-Origin Opener Policy
        Assert.True(middlewareConfiguration.UseCrossOriginOpenerPolicy);
        Assert.Equal("same-origin", middlewareConfiguration.CrossOriginOpenerPolicy.BuildHeaderValue());
    }
}
