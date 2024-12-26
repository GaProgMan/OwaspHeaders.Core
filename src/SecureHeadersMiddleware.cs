using System.Linq;
using System.Threading.Tasks;

namespace OwaspHeaders.Core;

/// <summary>
/// A middleware for injecting OWASP recommended headers into a
/// HTTP Request
/// </summary>
public class SecureHeadersMiddleware()
{
    private string _calculatedContentSecurityPolicy;
    private readonly Dictionary<string, string> _headers;
    private readonly RequestDelegate _next;
    private readonly SecureHeadersMiddlewareConfiguration _config;
    
    public SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config) : this()
    {
        _config = config;
        _next = next;
        _headers = new Dictionary<string, string>();
    }

    /// <summary>
    /// The main task of the middleware. This will be invoked whenever
    /// the middleware fires
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext" /> for the current request or response</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (_config == null)
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
        await _next(httpContext);
    }

    private void GenerateRelevantHeaders()
    {
        if (_config.UseHsts)
        {
            _headers.TryAdd(Constants.StrictTransportSecurityHeaderName,
                _config.HstsConfiguration.BuildHeaderValue());
        }

        if (_config.UseXFrameOptions)
        {
            _headers.TryAdd(Constants.XFrameOptionsHeaderName,
                _config.XFrameOptionsConfiguration.BuildHeaderValue());
        }

        if (_config.UseXssProtection)
        {
            _headers.TryAdd(Constants.XssProtectionHeaderName,
                _config.XssConfiguration.BuildHeaderValue());
        }

        if (_config.UseXContentTypeOptions)
        {
            _headers.TryAdd(Constants.XContentTypeOptionsHeaderName, Constants.XContentTypeOptionsValue);
        }

        if (_config.UseContentSecurityPolicyReportOnly)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy =
                    _config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue();
            }

            _headers.TryAdd(Constants.ContentSecurityPolicyReportOnlyHeaderName,
                _calculatedContentSecurityPolicy);
        }
        else if (_config.UseContentSecurityPolicy)
        {
            if (string.IsNullOrWhiteSpace(_calculatedContentSecurityPolicy))
            {
                _calculatedContentSecurityPolicy = _config.ContentSecurityPolicyConfiguration.BuildHeaderValue();
            }

            _headers.TryAdd(Constants.ContentSecurityPolicyHeaderName,
                _calculatedContentSecurityPolicy);
        }

        if (_config.UseXContentSecurityPolicy)
        {
            _headers.TryAdd(Constants.XContentSecurityPolicyHeaderName,
                _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        }

        if (_config.UsePermittedCrossDomainPolicy)
        {
            _headers.TryAdd(Constants.PermittedCrossDomainPoliciesHeaderName,
                _config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
        }

        if (_config.UseReferrerPolicy)
        {
            _headers.TryAdd(Constants.ReferrerPolicyHeaderName,
                _config.ReferrerPolicy.BuildHeaderValue());
        }

        if (_config.UseExpectCt)
        {
            _headers.TryAdd(Constants.ExpectCtHeaderName,
                _config.ExpectCt.BuildHeaderValue());
        }

        if (_config.UseCacheControl)
        {
            _headers.TryAdd(Constants.CacheControlHeaderName,
                _config.CacheControl.BuildHeaderValue());
        }

        if (_config.UseCrossOriginResourcePolicy)
        {
            _headers.TryAdd(Constants.CrossOriginResourcePolicyHeaderName,
                _config.CrossOriginResourcePolicy.BuildHeaderValue());
        }
    }

    private bool RequestShouldBeIgnored(PathString requestedPath)
    {
        if (_config.UrlsToIgnore.Count == 0)
        {
            return false;
        }

        return requestedPath.HasValue &&
               _config.UrlsToIgnore.Any(url => url.Equals(requestedPath.Value!, StringComparison.InvariantCulture));
    }
}
