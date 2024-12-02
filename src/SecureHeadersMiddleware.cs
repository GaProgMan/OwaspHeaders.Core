using System.Threading.Tasks;

namespace OwaspHeaders.Core
{
    /// <summary>
    /// A middleware for injecting OWASP recommended headers into a
    /// HTTP Request
    /// </summary>
    public class SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config)
    {
        private string _calculatedContentSecurityPolicy;

    /// <summary>
    /// The main task of the middleware. This will be invoked whenever
    /// the middleware fires
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext" /> for the current request or response</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (config == null)
        {
            throw new ArgumentException($"Expected an instance of the {nameof(SecureHeadersMiddlewareConfiguration)} object.");
        }

        if (config.UseHsts)
        {
            httpContext.TryAddHeader(Constants.StrictTransportSecurityHeaderName,
                config.HstsConfiguration.BuildHeaderValue());
        }

        if (config.UseXFrameOptions)
        {
            httpContext.TryAddHeader(Constants.XFrameOptionsHeaderName,
                config.XFrameOptionsConfiguration.BuildHeaderValue());
        }

        if (config.UseXssProtection)
        {
            httpContext.TryAddHeader(Constants.XssProtectionHeaderName,
                config.XssConfiguration.BuildHeaderValue());
        }

        if (config.UseXContentTypeOptions)
        {
            httpContext.TryAddHeader(Constants.XContentTypeOptionsHeaderName, "nosniff");
        }

        if (config.UseContentSecurityPolicyReportOnly)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy = config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue();
            }
            httpContext.TryAddHeader(Constants.ContentSecurityPolicyReportOnlyHeaderName,
                _calculatedContentSecurityPolicy);
        }
        else if (config.UseContentSecurityPolicy)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy = config.ContentSecurityPolicyConfiguration.BuildHeaderValue();
            }
            httpContext.TryAddHeader(Constants.ContentSecurityPolicyHeaderName,
                _calculatedContentSecurityPolicy);
        }

        if (config.UseXContentSecurityPolicy)
        {
            httpContext.TryAddHeader(Constants.XContentSecurityPolicyHeaderName,
                config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        }

        if (config.UsePermittedCrossDomainPolicy)
        {
            httpContext.TryAddHeader(Constants.PermittedCrossDomainPoliciesHeaderName,
                config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
        }

        if (config.UseReferrerPolicy)
        {
            httpContext.TryAddHeader(Constants.ReferrerPolicyHeaderName,
                config.ReferrerPolicy.BuildHeaderValue());
        }

        if (config.UseExpectCt)
        {
            httpContext.TryAddHeader(Constants.ExpectCtHeaderName,
                config.ExpectCt.BuildHeaderValue());
        }

        if (config.UseCacheControl)
        {
            httpContext.TryAddHeader(Constants.CacheControlHeaderName,
                config.CacheControl.BuildHeaderValue());
        }

        if (config.UseCrossOriginResourcePolicy)
        {
            httpContext.TryAddHeader(Constants.CrossOriginResourcePolicyHeaderName,
                config.CrossOriginResourcePolicy.BuildHeaderValue());
        }

        // Call the next middleware in the chain
        await next(httpContext);
    }
}
}
