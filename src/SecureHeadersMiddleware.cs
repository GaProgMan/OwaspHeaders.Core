using System.Collections.Frozen;
using System.Linq;
using System.Threading.Tasks;

namespace OwaspHeaders.Core;

/// <summary>
/// A middleware for injecting OWASP recommended headers into a
/// HTTP Request
/// </summary>
public class SecureHeadersMiddleware
{
    private FrozenDictionary<string, string> _headers;
    private readonly RequestDelegate _next;
    private readonly SecureHeadersMiddlewareConfiguration _config;

    public SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config)
    {
        _config = config;
        _next = next;
        _headers = FrozenDictionary<string, string>.Empty;
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
                _headers = GenerateRelevantHeaders();
            }

            foreach (var (key, value) in _headers)
            {
                httpContext.TryAddHeader(key, value);
            }
        }

        // Call the next middleware in the chain
        await _next(httpContext);
    }

    private FrozenDictionary<string, string> GenerateRelevantHeaders()
    {
        var temporaryDictionary = new Dictionary<string, string>();

        if (_config.UseHsts)
        {
            temporaryDictionary.Add(Constants.StrictTransportSecurityHeaderName,
                _config.HstsConfiguration.BuildHeaderValue());
        }

        if (_config.UseXFrameOptions)
        {
            temporaryDictionary.Add(Constants.XFrameOptionsHeaderName,
                _config.XFrameOptionsConfiguration.BuildHeaderValue());
        }

        if (_config.UseXssProtection)
        {
            temporaryDictionary.Add(Constants.XssProtectionHeaderName,
                _config.XssConfiguration.BuildHeaderValue());
        }

        if (_config.UseXContentTypeOptions)
        {
            temporaryDictionary.Add(Constants.XContentTypeOptionsHeaderName, Constants.XContentTypeOptionsValue);
        }

        if (_config.UseContentSecurityPolicyReportOnly)
        {
            temporaryDictionary.Add(Constants.ContentSecurityPolicyReportOnlyHeaderName,
                _config.ContentSecurityPolicyReportOnlyConfiguration.BuildHeaderValue());
        }
        else if (_config.UseContentSecurityPolicy)
        {
            temporaryDictionary.Add(Constants.ContentSecurityPolicyHeaderName,
                _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        }

        if (_config.UseXContentSecurityPolicy)
        {
            temporaryDictionary.Add(Constants.XContentSecurityPolicyHeaderName,
                _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
        }

        if (_config.UsePermittedCrossDomainPolicy)
        {
            temporaryDictionary.Add(Constants.PermittedCrossDomainPoliciesHeaderName,
                _config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
        }

        if (_config.UseReferrerPolicy)
        {
            temporaryDictionary.Add(Constants.ReferrerPolicyHeaderName,
                _config.ReferrerPolicy.BuildHeaderValue());
        }

        if (_config.UseExpectCt)
        {
            temporaryDictionary.Add(Constants.ExpectCtHeaderName,
                _config.ExpectCt.BuildHeaderValue());
        }

        if (_config.UseCacheControl)
        {
            temporaryDictionary.Add(Constants.CacheControlHeaderName,
                _config.CacheControl.BuildHeaderValue());
        }

        if (_config.UseCrossOriginResourcePolicy)
        {
            temporaryDictionary.Add(Constants.CrossOriginResourcePolicyHeaderName,
                _config.CrossOriginResourcePolicy.BuildHeaderValue());
        }

        if (_config.UseCrossOriginOpenerPolicy)
        {
            temporaryDictionary.Add(Constants.CrossOriginOpenerPolicyHeaderName,
                _config.CrossOriginOpenerPolicy.BuildHeaderValue());
        }
        
        if (_config.UseCrossOriginEmbedderPolicy)
        {
            if (!_config.UseCrossOriginResourcePolicy)
            {
                BoolValueGuardClauses.MustBeTrue(_config.UseCrossOriginResourcePolicy, nameof(_config.UseCrossOriginResourcePolicy));
            }
            temporaryDictionary.Add(Constants.CrossOriginEmbedderPolicyHeaderName,
                _config.CrossOriginEmbedderPolicy.BuildHeaderValue());
        }

        return temporaryDictionary.ToFrozenDictionary();
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
