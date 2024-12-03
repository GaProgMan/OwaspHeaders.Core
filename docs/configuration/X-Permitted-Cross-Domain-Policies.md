---
title: X-Permitted-Cross-Domain-Policies
nav_order: 5
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the X-Permitted-Cross-Domain-Policies header like this:

{: .quote }
> Specifies if a cross-domain policy file (crossdomain.xml) is allowed. The file may define a policy to grant clients, such as Adobe's Flash Player (now obsolete), Adobe Acrobat, Microsoft Silverlight (now obsolete), or Apache Flex, permission to handle data across domains that would otherwise be restricted due to the Same-Origin Policy.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers

{: .note} 
> It is worth noting that the X-Permitted-Cross-Domain-Policies header was originally created when browsers would make use of Adobe's Flash Player, Adobe Acrobat, Microsoft Silverlight, or Apache Flex and that two of those technologies are now obsolete.
> 
> Whilst those technologies are obsolete, it's still worth having this header present as a "belt-and-braces" check against any future vulnerabilities which might mask themselves as any of the obsolete technologies it was invented to protect.

An X-Permitted-Cross-Domain-Policies header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the X-Permitted-Cross-Domain-Policies header with a `none` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UsePermittedCrossDomainPolicies()
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
```

The above adds the X-Permitted-Cross-Domain-Policies header with a `none` value.

## Full Options

The X-Permitted-Cross-Domain-Policies header object (known internally as `PermittedCrossDomainPolicyConfiguration`) has the following options:

- enum: `XPermittedCrossDomainOptionValue`

The values available for the `XPermittedCrossDomainOptionValue` enum are:

- `none`
- `masterOnly`
- `byContentType`
- `byFtpFileType`
- `all`

These values can be set when creating a new instance of the `XPermittedCrossDomainOptionValue` object, or by calling the `UsePermittedCrossDomainPolicies` extension method on the `SecureHeadersMiddlewareConfiguration` class.
