---
title: Configuration
nav_order: 2
layout: page
---

OwaspHeaders.Core uses the builder pattern to set up the header information, which is a compile time dependency. If you need
to change the configuration, you will need to rebuild your application. This is an intentional design choice, as a change
to any of the HTTP headers that the OwaspHeaders.Core middleware injects should require the application to restart.

## Basic Configuration

Add the default configuration by adding the following to your `Program.cs` the Middleware pipeline as early as is
possible for your application design:

```csharp
app.UseSecureHeadersMiddleware();
```

The above will use the default configuration for the OwaspHeaders.Core middleware. The method that sets the defaults looks
like this:

```csharp
public static SecureHeadersMiddlewareConfiguration BuildDefaultConfiguration() 
{ 
    return SecureHeadersMiddlewareBuilder 
        .CreateBuilder() 
        .UseHsts() 
        .UseXFrameOptions() 
        .UseContentTypeOptions() 
        .UseContentDefaultSecurityPolicy() 
        .UsePermittedCrossDomainPolicies() 
        .UseReferrerPolicy() 
        .UseCacheControl() 
        .RemovePoweredByHeader() 
        .UseXssProtection() 
        .UseCrossOriginResourcePolicy() 
        .Build(); 
} 
```

{: .warning }
The default configuration is INCREDIBLY restrictive.

The following is an example of the response headers from version 9.1.0 (taken on November 19th, 2024) when using the
default configuration: 

```plaintext
Cache-Control: max-age=31536000;private
Strict-Transport-Security: max-age=31536000;includeSubDomains
X-Frame-Options: DENY
X-XSS-Protection: 0
X-Content-Type-Options: nosniff
Content-Security-Policy: script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;
X-Permitted-Cross-Domain-Policies: none;
Referrer-Policy: no-referrer
Cross-Origin-Resource-Policy: same-origin
```

{: .note }
The above example contains only the headers added by the Middleware.

### Custom Configuration

In most cases (except for the [Content-Security Policy](./Content-Security-Policy)), the default configuration will
be suitable. This is because it adds the OWASP recommended headers and values. Content-Security Policy is a non-trivial
header, and is an allowlist for sources of content for the rendered page.

In some cases, you may need to provide a custom configuration for the OwaspHeaders.Core middleware. In order to use a
custom configuration, follow the same pattern.

{: .suggestion }
We recommend creating your own extension method to encapsulate your custom configuration.

In the following example, we've created a static method called `CustomConfiguration` within a static extensions class
(called `CustomSecureHeaderExtensions`). This custom method returns an instance of the `SecureHeadersMiddlewareConfiguration`
which contains all the configuration required for a fictional custom configuration:

``` csharp
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Example.Helpers;

public static class CustomSecureHeaderExtensions
{
    public static SecureHeadersMiddlewareConfiguration CustomConfiguration()
    {
        return SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts(1200, false)
            .UseContentDefaultSecurityPolicy()
            .UsePermittedCrossDomainPolicies(XPermittedCrossDomainOptionValue.masterOnly)
            .UseReferrerPolicy(ReferrerPolicyOptions.sameOrigin)
            .Build();
    }
}
```

{: .note }
This is an example configuration. It is recommended that you do NOT use this configuration in a production environment.

Then consume it in the following manner, within your `Program.cs`'s Middleware pipeline:

```csharp
app.UseSecureHeadersMiddleware(
    CustomSecureHeaderExtensions.CustomConfiguration()
);
```

This configuration will add the following headers to all server-generated responses:

```plaintext
Strict-Transport-Security: max-age=1200
Content-Security-Policy: script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;
X-Permitted-Cross-Domain-Policies: master-only;
Referrer-Policy: same-origin
```

{: .note }
The above example contains only the headers added by the Middleware for the configuration provided in the
`CustomConfiguration` extension method.
