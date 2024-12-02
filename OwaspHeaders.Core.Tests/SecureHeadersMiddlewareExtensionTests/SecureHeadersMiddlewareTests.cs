using Microsoft.AspNetCore.Builder;
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
        var response = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration();

        // Assert
        Assert.NotNull(response);
        Assert.IsType<SecureHeadersMiddlewareConfiguration>(response);

        // HSTS
        Assert.True(response.UseHsts);
        Assert.Equal("max-age=31536000;includeSubDomains", response.HstsConfiguration.BuildHeaderValue());

        // X-Frame-Options
        Assert.True(response.UseXFrameOptions);
        Assert.Equal("DENY", response.XFrameOptionsConfiguration.BuildHeaderValue());

        // X-Content-Type-Options
        Assert.True(response.UseXContentTypeOptions);
        // Can't easily assert the value here, as it's set in the InvokeAsync for the middleware

        // Content-Security-Policy
        Assert.True(response.UseContentSecurityPolicy);
        Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;",
            response.ContentSecurityPolicyConfiguration.BuildHeaderValue());

        // X-Permitted-Cross-Domain-Policies
        Assert.True(response.UsePermittedCrossDomainPolicy);
        Assert.Equal("none;", response.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());

        // Referrer-Policy
        Assert.True(response.UseReferrerPolicy);
        Assert.Equal("no-referrer", response.ReferrerPolicy.BuildHeaderValue());

        // Cache-Control
        Assert.True(response.UseCacheControl);
        Assert.Equal("max-age=31536000, private", response.CacheControl.BuildHeaderValue());

        // X-XSS-Protection
        Assert.True(response.UseXssProtection);
        Assert.Equal("0", response.XssConfiguration.BuildHeaderValue());

        // Cross-Origin Resource Policy
        Assert.True(response.UseCrossOriginResourcePolicy);
        Assert.Equal("same-origin", response.CrossOriginResourcePolicy.BuildHeaderValue());
    }
}
