using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core
{
    /// <summary>
    /// A middleware for injecting OWASP recommended headers into a
    /// HTTP Request
    /// </summary>
    public class SecureHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecureHeadersMiddlewareConfiguration _config;

        public SecureHeadersMiddleware(RequestDelegate next, SecureHeadersMiddlewareConfiguration config)
        {
            _next = next;
            _config = config;
        }

        /// <summary>
        /// The main task of the middleware. This will be invoked whenever
        /// the middleware fires
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext" /> for the current request or response</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (_config == null)
            {
                throw new ArgumentException($@"Expected an instance of the
                        {nameof(SecureHeadersMiddlewareConfiguration)} object.");
            }

            if (_config.UseHsts)
            {
                httpContext.TryAddHeader(Constants.StrictTransportSecurityHeaderName,
                    _config.HstsConfiguration.BuildHeaderValue());
            }

            if (_config.UseXFrameOptions)
            {
                httpContext.TryAddHeader(Constants.XFrameOptionsHeaderName,
                    _config.XFrameOptionsConfiguration.BuildHeaderValue());
            }

            if (_config.UseXssProtection)
            {
                httpContext.TryAddHeader(Constants.XssProtectionHeaderName,
                    _config.XssConfiguration.BuildHeaderValue());
            }

            if (_config.UseXContentTypeOptions)
            {
                httpContext.TryAddHeader(Constants.XContentTypeOptionsHeaderName, "nosniff");
            }

            if (_config.UseContentSecurityPolicy)
            {
                httpContext.TryAddHeader(Constants.ContentSecurityPolicyHeaderName,
                    _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
            }

            if (_config.UseXContentSecurityPolicy)
            {
                httpContext.TryAddHeader(Constants.XContentSecurityPolicyHeaderName,
                _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
            }

            if (_config.UsePermittedCrossDomainPolicy)
            {
                httpContext.TryAddHeader(Constants.PermittedCrossDomainPoliciesHeaderName,
                    _config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
            }

            if (_config.UseReferrerPolicy)
            {
                httpContext.TryAddHeader(Constants.ReferrerPolicyHeaderName,
                    _config.ReferrerPolicy.BuildHeaderValue());
            }

            if (_config.UseExpectCt)
            {
                httpContext.TryAddHeader(Constants.ExpectCtHeaderName,
                    _config.ExpectCt.BuildHeaderValue());
            }

            if (_config.RemoveXPoweredByHeader)
            {
                httpContext.TryRemoveHeader(Constants.PoweredByHeaderName);
            }

            // Call the next middleware in the chain
            await _next.Invoke(httpContext);
        }
    }
}
