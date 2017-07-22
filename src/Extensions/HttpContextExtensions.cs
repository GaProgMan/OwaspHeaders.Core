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
            if (httpContext.ResponseContainsHeader(headerName)) return true;
            try
            {
                httpContext.Response.Headers.Add(headerName, headerValue);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}