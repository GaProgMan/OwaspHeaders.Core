---
title: X-XSS-Protection
nav_order: 3
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the X-XSS-Protection header like this:

{: .quote }
> The HTTP X-XSS-Protection response header was a feature of Internet Explorer, Chrome and Safari that stopped pages from loading when they detected reflected cross-site scripting (XSS) attacks. These protections are largely unnecessary in modern browsers when sites implement a strong Content-Security-Policy that disables the use of inline JavaScript ('unsafe-inline').
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection

Both the OWASP Secure Headers Project and MDN recommend not using this header with any value other than "0", which disabled the XSS Auditor. This is due to the X-XSS-Protection header having been dropped from most modern browsers and that using it (with a value other than "0") can cause additional security issues to present themselves. The recommended path forward is to use a [Content-Security-Policy (CSP)](Content-Security-Policy.md) header.

As such, the only value that OwaspHeaders.Core supports for the X-XSS-Protection header is "0", and the header can be added in one of two ways:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the X-XSS-Protection header with a "0" value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customHstsConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseXssProtection()
    .Build();

app.UseSecureHeadersMiddleware(customHstsConfig);
```

The above adds the X-XSS-Protection header with a "0" value.

{: .note }
The API for OwaspHeaders.Core does not support adding a value other than "0" for the X-XSS-Protection header.

## Full Options

There are no options for the X-XSS-Protection header.
