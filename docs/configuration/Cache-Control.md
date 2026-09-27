---
title: Cache-Control
nav_order: 8
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Cache-Control header like this:

{: .quote }
> The HTTP Cache-Control header holds directives (instructions) in both requests and responses that control caching in browsers and shared caches (e.g., Proxies, CDNs).
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control

A Cache-Control header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the Cache-Control header with a `no-cache, no-store, max-age=0` value.

Or by passing a configure delegate to `UseSecureHeadersMiddleware`, which is handed a `SecureHeadersBuilder`:

```csharp
app.UseSecureHeadersMiddleware(opt => opt.UseCacheControl());
```

The above adds the Cache-Control header with a `no-cache, no-store, max-age=0` value.

## Full Options

The Cache-Control header object (known internally as `CacheControl`) has the following options:

| Option | Type | Default | Directive |
|---|---|---|---|
| `Private` | bool | `false` | `private` |
| `NoCache` | bool | `true` | `no-cache` |
| `NoStore` | bool | `true` | `no-store` |
| `MaxAge` | int | `0` | `max-age=<value>` (always included; a negative value is sent as `0`) |
| `MustRevalidate` | bool | `false` | `must-revalidate` |

These values are set by calling the `UseCacheControl` extension method on the `SecureHeadersMiddlewareConfiguration` class.

Every directive whose option is set is included in the header, in the order shown above. For example:

```csharp
app.UseSecureHeadersMiddleware(opt =>
    opt.UseCacheControl(@private: true, maxAge: 60, noCache: false, noStore: false));
```

The above adds the Cache-Control header with a `private, max-age=60` value.

{: .warning }
> As `NoCache` defaults to `true`, allowing caching requires setting both `noCache: false` and `noStore: false`. For example, `UseCacheControl(maxAge: 3600, noStore: false)` produces `no-cache, max-age=3600`, which requires caches to revalidate the response before every reuse and so effectively disables the `max-age`.

{: .note }
> ASP.NET Core's antiforgery system (used by any Razor page or MVC view containing a `<form method="post">`) requires both `no-cache` and `no-store` on responses which include an antiforgery token. If either is missing, it overrides the Cache-Control header and logs a warning. The default values include both. Prior to version 10.5.0 the default value was `max-age=0,no-store`, and setting one option discarded the others; see [issue #261](https://github.com/GaProgMan/OwaspHeaders.Core/issues/261).

{: .warning }
> It's worth noting that the default values for this header mean that no content will be cached in the browser. You may need to evaluate this default value on a case-by-case basis. 
