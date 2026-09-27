---
title: Referrer-Policy
nav_order: 6
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Referrer-Policy header like this:

{: .quote }
> The HTTP Referrer-Policy response header controls how much referrer information (sent with the Referer header) should be included with requests.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy

A Referrer-Policy header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the Referrer-Policy header with a `no-referrer` value.

Or by passing a configure delegate to `UseSecureHeadersMiddleware`, which is handed a `SecureHeadersBuilder`:

```csharp
app.UseSecureHeadersMiddleware(opt => opt.UseReferrerPolicy());
```

The above adds the Referrer-Policy header with a `no-referrer` value.

## Full Options

The Referrer-Policy header object (known internally as `ReferrerPolicy`) has the following options:

- enum: `ReferrerPolicyOptions`

The values available for the `ReferrerPolicyOptions` enum are:

- `noReferrer`
- `noReferrerWhenDowngrade`
- `origin`
- `originWhenCrossOrigin`
- `sameOrigin`
- `strictOrigin`
- `strictWhenCrossOrigin`
- `unsafeUrl`

These values are set by calling the `UseReferrerPolicy` extension method on the `SecureHeadersMiddlewareConfiguration` class.
