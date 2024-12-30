---
title: Cross-Origin-Opener-Policy
nav_order: 9
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Cross-Origin-Opener-Policy (COOP) header like this:

{: .quote }
> The HTTP Cross-Origin-Opener-Policy (COOP) response header allows a website to control whether a new top-level
> document, opened using Window.open() or by navigating to a new page, is opened in the same browsing context group
> (BCG) or in a new browsing context group.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Opener-Policy

A COOP header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the COOP header with a `same-origin` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseCrossOriginOpenerPolicy()
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
```

The above adds the COOP header with a `same-origin` value.

## Full Options

The COOP header object (known internally as `CrossOriginOpenerPolicy`) has the following options:

- enum: `CrossOriginOpenerOptions`

The values available for the `CrossOriginOpenerOptions` enum are:

- `UnsafeNone`
- `SameOriginAllowPopups`
- `SameOrigin`

