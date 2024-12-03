---
title: Cache-Control
nav_order: 7
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

The above adds the Cache-Control header with a `no-store, max-age=0` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseCacheControl()
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
```

The above adds the Cache-Control header with a `no-store, max-age=0` value.

## Full Options

The Cache-Control header object (known internally as `CacheControl`) has the following options:

- bool: `Private`
- int: `MaxAge`
- bool: `NoCache`
- bool: `MustReevaluate`
- bool: `NoStore`

These values can be set when creating a new instance of the `ReferrerPolicyOptions` object, or by calling the `UseCacheControl` extension method on the `SecureHeadersMiddlewareConfiguration` class.

{: .warning }
> It's worth noting that the default values for this header mean that no content will be cached in the browser. You may need to evaluate this default value on a case-by-case basis. 
