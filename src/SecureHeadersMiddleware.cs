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
    private readonly ILogger<SecureHeadersMiddleware> _logger;

    public SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config,
        ILogger<SecureHeadersMiddleware> logger = null)
    {
        _config = config;
        _next = next;
        _logger = logger;
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
            var errorMessage = $"Expected an instance of the {nameof(SecureHeadersMiddlewareConfiguration)} object.";
            LogConfigurationError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        if (!RequestShouldBeIgnored(httpContext.Request.Path))
        {
            if (_headers.Count == 0)
            {
                _headers = GenerateRelevantHeaders();
                LogMiddlewareInitialized(_headers.Count);
                LogHeadersGenerated(_headers.Count);
            }

            var totalHeadersAdded = 0;

            foreach (var (key, value) in _headers)
            {
                var headerFailedEventId = _config?.LoggingConfiguration?.HeaderAdditionFailed ?? SecureHeadersEventIds.HeaderAdditionFailed;
                if (httpContext.TryAddHeader(key, value, _logger, headerFailedEventId))
                {
                    LogHeaderAdded(key, value.Length);
                    totalHeadersAdded++;
                }
            }

            // Handle path-specific Clear-Site-Data header
            if (_config.UseClearSiteData && _config.ClearSiteDataPathConfiguration != null)
            {
                var clearSiteDataConfig = _config.ClearSiteDataPathConfiguration
                    .GetConfigurationForPath(httpContext.Request.Path.Value);

                if (clearSiteDataConfig != null)
                {
                    var clearSiteDataValue = clearSiteDataConfig.BuildHeaderValue();
                    var headerFailedEventId = _config?.LoggingConfiguration?.HeaderAdditionFailed ?? SecureHeadersEventIds.HeaderAdditionFailed;

                    if (httpContext.TryAddHeader(Constants.ClearSiteDataHeaderName, clearSiteDataValue, _logger, headerFailedEventId))
                    {
                        LogHeaderAdded(Constants.ClearSiteDataHeaderName, clearSiteDataValue.Length);
                        totalHeadersAdded++;
                    }
                }
            }

            LogHeadersAdded(totalHeadersAdded, httpContext.Request.Path.Value ?? "");
        }
        else
        {
            LogRequestIgnored(httpContext.Request.Path.Value ?? "");
        }

        // Call the next middleware in the chain
        await _next(httpContext);
    }

    #region Logging Methods

    private void LogMiddlewareInitialized(int headerCount)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Information))
        {
            var eventId = _config?.LoggingConfiguration?.MiddlewareInitialized ?? SecureHeadersEventIds.MiddlewareInitialized;
            _logger.Log(LogLevel.Information,
                eventId,
                "SecureHeaders middleware initialized with {HeaderCount} headers enabled",
                headerCount);
        }
    }

    private void LogHeadersAdded(int headerCount, string requestPath)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Information))
        {
            var eventId = _config?.LoggingConfiguration?.HeadersAdded ?? SecureHeadersEventIds.HeadersAdded;
            _logger.Log(LogLevel.Information,
                eventId,
                "Added {HeaderCount} security headers to response for {RequestPath}",
                headerCount, requestPath);
        }
    }

    private void LogRequestIgnored(string requestPath)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Information))
        {
            var eventId = _config?.LoggingConfiguration?.RequestIgnored ?? SecureHeadersEventIds.RequestIgnored;
            _logger.Log(LogLevel.Information,
                eventId,
                "Request ignored due to URL exclusion rule: {RequestPath}",
                requestPath);
        }
    }

    private void LogHeadersGenerated(int headerCount)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Information))
        {
            var eventId = _config?.LoggingConfiguration?.HeadersGenerated ?? SecureHeadersEventIds.HeadersGenerated;
            _logger.Log(LogLevel.Information,
                eventId,
                "Generated {HeaderCount} security headers",
                headerCount);
        }
    }

    private void LogHeaderAdded(string headerName, int valueLength)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
        {
            var eventId = _config?.LoggingConfiguration?.HeaderAdded ?? SecureHeadersEventIds.HeaderAdded;
            _logger.Log(LogLevel.Debug,
                eventId,
                "Added header {HeaderName} with value length {ValueLength}",
                headerName, valueLength);
        }
    }

    private void LogConfigurationError(string validationError)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Error))
        {
            var eventId = _config?.LoggingConfiguration?.ConfigurationError ?? SecureHeadersEventIds.ConfigurationError;
            _logger.Log(LogLevel.Error,
                eventId,
                "Configuration validation failed: {ValidationError}",
                validationError);
        }
    }

    private void LogConfigurationIssue(string issue)
    {
        if (_logger != null && _logger.IsEnabled(LogLevel.Warning))
        {
            var eventId = _config?.LoggingConfiguration?.ConfigurationIssue ?? SecureHeadersEventIds.ConfigurationIssue;
            _logger.Log(LogLevel.Warning,
                eventId,
                "Configuration issue detected: {Issue}",
                issue);
        }
    }

    #endregion

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
            if (!_config.CrossOriginEmbedderPolicy.HeaderValueIsValid(_config.UseCrossOriginResourcePolicy))
            {
                LogConfigurationIssue(
                    "Cross-Origin-Embedder-Policy requires Cross-Origin-Resource-Policy to be enabled");
                BoolValueGuardClauses.MustBeTrue(_config.UseCrossOriginResourcePolicy,
                    nameof(_config.UseCrossOriginResourcePolicy));
            }

            temporaryDictionary.Add(Constants.CrossOriginEmbedderPolicyHeaderName,
                _config.CrossOriginEmbedderPolicy.BuildHeaderValue());
        }

        if (_config.UseReportingEndPoints)
        {
            temporaryDictionary.Add(Constants.ReportingEndpointsHeaderName,
                _config.ReportingEndpointsPolicy.BuildHeaderValue());
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
