using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
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
        /// <param name="HttpContext">The <see cref="HttpContext" /> for the current request or response</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (_config.UseHsts && !httpContext.ResponseContainsHeader(Constants.StrictTransportSecurityHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.StrictTransportSecurityHeaderName,
                    _config.HstsConfiguration.BuildHeaderValue());
            }

            if (_config.UseHpkp && !httpContext.ResponseContainsHeader(Constants.PublicKeyPinsHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.PublicKeyPinsHeaderName,
                    _config.HpkpConfiguration.BuildHeaderValue());
            }

            if (_config.UseXFrameOptions && !httpContext.ResponseContainsHeader(Constants.XFrameOptionsHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.XFrameOptionsHeaderName,
                    _config.XFrameOptionsConfiguration.BuildHeaderValue());
            }

            if (_config.UseXssProtection && !httpContext.ResponseContainsHeader(Constants.XssProtectionHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.XssProtectionHeaderName,
                    _config.XssConfiguration.BuildHeaderValue());
            }

            if (_config.UseXContentTypeOptions &&
                !httpContext.ResponseContainsHeader(Constants.XContentTypeOptionsHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.XContentTypeOptionsHeaderName, "nosniff");
            }

            if (_config.UseContentSecurityPolicy &&
                !httpContext.ResponseContainsHeader(Constants.ContentSecurityPolicyHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.ContentSecurityPolicyHeaderName,
                    _config.ContentSecurityPolicyConfiguration.BuildHeaderValue());
            }
            if (_config.UsePermittedCrossDomainPolicy &&
                !httpContext.ResponseContainsHeader(Constants.PermittedCrossDomainPoliciesHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.PermittedCrossDomainPoliciesHeaderName,
                    _config.PermittedCrossDomainPolicyConfiguration.BuildHeaderValue());
            }

            if (_config.UseReferrerPolicy && !httpContext.ResponseContainsHeader(Constants.ReferrerPolicyHeaderName))
            {
                httpContext.Response.Headers.Add(Constants.ReferrerPolicyHeaderName,
                    _config.ReferrerPolicy.BuildHeaderValue());
            }

            // Call the next middleware in the chain
            await _next.Invoke(httpContext);
        }
    }
}
