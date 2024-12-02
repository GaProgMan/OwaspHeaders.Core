---
title: X-Frame-Options (XFO)
nav_order: 2
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the X-Frame-Options header like this:

{: .quote }
> The X-Frame-Options HTTP response header can be used to indicate whether a browser should be allowed to render a page in a <frame>, <iframe>, <embed> or <object>. Sites can use this to avoid click-jacking attacks, by ensuring that their content is not embedded into other sites.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options

It's worth noting that the MDN states that the X-Frame-Options header has been deprecated and removed from most browsers, the OWASP Secure Headers Project still recommends it's use. The MDN-recommended replacement for X-Frame-Options is to use the `frame-ancestors` directive in a [Content-Security-Policy (CSP)](Content-Security-Policy.md) header. However, it's entirely possible to use this middleware without including a CSP header; which is one of the reasons for OWASP maintaining their recommendation on using the X-Frame-Options header.

An X-Frame-Options header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the X-Frame-Options header with a `deny` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customHstsConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseXFrameOptions(XFrameOptions.Sameorigin)
    .Build();

app.UseSecureHeadersMiddleware(customHstsConfig);
```

The above adds the X-Frame-Options header with a `Sameorigin` value.

{: .note }
This allows any <frame>, <iframe>, <embed> or <object> elements to be included on a page if the page has the same origin domain as the element's target.

## Full Options

The X-Frame-Options header object (known internally as `UseXFrameOptions`) has the following options:

- enum: `XFrameOptions`

The values available for the `XFrameOptions` enum are:

- `Deny`
- `Sameorigin`

These values can be set when creating a new instance of the `HstsConfiguration` object, or by calling the `UseHsts` extension method on the `SecureHeadersMiddlewareConfiguration` class.

