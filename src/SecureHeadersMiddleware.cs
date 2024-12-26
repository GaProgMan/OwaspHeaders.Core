using System.Linq;
using System.Threading.Tasks;

namespace OwaspHeaders.Core;

/// <summary>
/// A middleware for injecting OWASP recommended headers into a
/// HTTP Request
/// </summary>
public class SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config)
{
    private string _calculatedContentSecurityPolicy;
    private readonly Dictionary<string, string> _headers = new();

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
            throw new ArgumentException(
                $"Expected an instance of the {nameof(SecureHeadersMiddlewareConfiguration)} object.");
        }

        if (!RequestShouldBeIgnored(httpContext.Request.Path))
        {
            if (_headers.Count == 0)
            {
                GenerateRelevantHeaders();
            }

            foreach (var (key, value) in _headers)
            {
                httpContext.TryAddHeader(key, value);
            }
        }

        // Call the next middleware in the chain
        await next(httpContext);
    }

    private void GenerateRelevantHeaders()
    {
        if (config.UseHsts)
        {
            _headers.TryAdd(Constants.StrictTransportSecurityHeaderName,
                config.HstsConfiguration.BuildHeaderValue());
        }

        if (config.UseXFrameOptions)
        {
            _headers.TryAdd(Constants.XFrameOptionsHeaderName,
                config.XFrameOptionsConfiguration.BuildHeaderValue());
        }

        if (config.UseXssProtection)
        {
            _headers.TryAdd(Constants.XssProtectionHeaderName,
                config.XssConfiguration.BuildHeaderValue());
        }

        if (config.UseXContentTypeOptions)
        {
            _headers.TryAdd(Constants.XContentTypeOptionsHeaderName, Constants.XContentTypeOptionsValue);
        }

        if (config.UseContentSecurityPolicyReportOnly)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy =
                    config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue();
            }

            _headers.TryAdd(Constants.ContentSecurityPolicyReportOnlyHeaderName,
                _calculatedContentSecurityPolicy);
        }
        else if (config.UseContentSecurityPolicy)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy = config.ContentSecurityPolicyConfiguration.BuildHeaderValue();
            }

            _headers.TryAdd(Constants.ContentSecurityPolicyHeaderName,
                _calculatedContentSecurityPolicy);
        }

        if (config.UseXContentSecurityPolicy)
        {
            _headers.TryAdd(Constants.XContentSecurityPolicyHeaderName,
                config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        }

        if (config.UsePermittedCrossDomainPolicy)
        {
            _headers.TryAdd(Constants.PermittedCrossDomainPoliciesHeaderName,
                config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
        }

        if (config.UseReferrerPolicy)
        {
            _headers.TryAdd(Constants.ReferrerPolicyHeaderName,
                config.ReferrerPolicy.BuildHeaderValue());
        }

        if (config.UseExpectCt)
        {
            _headers.TryAdd(Constants.ExpectCtHeaderName,
                config.ExpectCt.BuildHeaderValue());
        }

        if (config.UseCacheControl)
        {
            _headers.TryAdd(Constants.CacheControlHeaderName,
                config.CacheControl.BuildHeaderValue());
        }

        if (config.UseCrossOriginResourcePolicy)
        {
            _headers.TryAdd(Constants.CrossOriginResourcePolicyHeaderName,
                config.CrossOriginResourcePolicy.BuildHeaderValue());
        }
    }

    private bool RequestShouldBeIgnored(PathString requestedPath)
    {
        if (config.UrlsToIgnore.Count == 0)
        {
            return false;
        }

        return requestedPath.HasValue &&
               config.UrlsToIgnore.Any(url => url.Equals(requestedPath.Value!, StringComparison.InvariantCulture));
    }
}
