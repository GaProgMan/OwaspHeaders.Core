using System.Collections.Generic;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Extensions
{
    public static class ContentSecurityPolicyExtensions
    {
        /// <summary>
        /// Used to set the Content Security Policy URIs for a given <see cref="CspUriType"/>
        /// </summary>
        public static SecureHeadersMiddlewareConfiguration SetCspUris
            (this SecureHeadersMiddlewareConfiguration config, List<ContentSecurityPolicyElement> baseUri,
            CspUriType cspUriType)
        {
            if (config.UseContentSecurityPolicy)
            {
                config.ContentSecurityPolicyConfiguration?.SetCspUri(baseUri, cspUriType);
            }
            
            return config;
        }

        /// <summary>
        /// Used to set up the Content Security Policy Sandbox for a given or multiple <see cref="CspSandboxType"/>s
        /// </summary>
        public static SecureHeadersMiddlewareConfiguration SetCspSandBox
            (this SecureHeadersMiddlewareConfiguration config, params CspSandboxType[] sandboxType)
        {
            if (config.UseContentSecurityPolicy)
            {
                config.ContentSecurityPolicyConfiguration?.SetSandbox(sandboxType);
            }
            
            return config;
        }
    }
}