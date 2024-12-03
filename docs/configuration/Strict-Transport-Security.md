---
title: Strict-Transport-Security (HSTS)
nav_order: 1
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Strict-Transport-Security header like this:

{: .quote }
> The HTTP Strict Transport Security header informs the browser that it should never load a site using HTTP and should automatically convert all attempts to access the site using HTTP to HTTPS requests instead.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security#description

An HSTS header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the HSTS header with the following values:

| Directive | Value    |
|-----------|----------|
| max-age   | 31536000 |
| includeSubDomains | (no value needed) |

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts(1200, false)
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
```

The above adds the HSTS header with the following values:

| Directive | Value    |
|-----------|----------|
| max-age   | 1200     |

{: .note }
The above example does not enforce the inclusion of subdomains; as such the HSTS header will only be applied at the domain level.

## Full Options

The HSTS header object (known internally as `HstsConfiguration`) has the following options:

- int: `maxAge`
- bool: `includeSubdomains`

These values can be set when creating a new instance of the `HstsConfiguration` object, or by calling the `UseHsts` extension method on the `SecureHeadersMiddlewareConfiguration` class.
