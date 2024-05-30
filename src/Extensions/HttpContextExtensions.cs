using System;
using Microsoft.AspNetCore.Http;

namespace OwaspHeaders.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool ResponseContainsHeader(this HttpContext httpContext, string header)
        {
            return httpContext.Response.Headers.ContainsKey(header);
        }

        public static bool TryAddHeader(this HttpContext httpContext, string headerName, string headerValue)
        {
            if (httpContext.ResponseContainsHeader(headerName))
            {
                return true;
            }
            try
            {
                // ASP0019 states that:
                // "IDictionary.Add will throw an ArgumentException when attempting to add a duplicate key."
                // However, we've already done a check to see whether the
                // Response.Headers object
                // already contains a header with this name (in the above if statement).
                // So we'll disable the warning here then immediately restore it
                // after we've done what we need to.
                #pragma warning disable ASP0019
                httpContext.Response.Headers.Append(headerName, headerValue);
                #pragma warning restore ASP0019
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Used to remove a header (supplied via <see cref="headerName"/>) from the current
        /// <see cref="httpContext"/>
        /// </summary>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <param name="headerName">The name of the HTTP header to remove</param>
        /// <returns></returns>
        public static bool TryRemoveHeader(this HttpContext httpContext, string headerName)
        {
            if (!httpContext.ResponseContainsHeader(headerName))
            {
                return true;
            }
            try
            {
                httpContext.Response.Headers.Remove(headerName);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
