using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
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
            if(_config.UseHsts)
            {
                httpContext.Response.Headers.Add("Strict-Transport-Security", _config.HstsConfiguration.BuilderHeaderValue());
            }

            if (_config.UseHpkp)
            {
                httpContext.Response.Headers.Add("Public-Key-Pins", _config.HpkpConfiguration.BuildHeaderValue());
            }
            
            if (_config.UseXFrameOptions)
            {
                httpContext.Response.Headers.Add("X-Frame-Options", _config.XFrameOptionsConfiguration.BuildHeaderValue());
            }

            if (_config.UseXssProtection)
            {
                httpContext.Response.Headers.Add("X-XSS-Protection", _config.XssConfiguration.BuildHeaderValue());
            }

            // TODO implement these https://www.owasp.org/index.php/OWASP_Secure_Headers_Project#tab=Headers - X-Content-Type-Options next

            // Call the next middleware in the chain
            await _next.Invoke(httpContext);
        }
    }
}
