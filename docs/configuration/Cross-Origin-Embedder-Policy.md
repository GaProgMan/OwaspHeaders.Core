---
title: Cross-Origin-Opener-Policy
nav_order: 10
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Cross-Origin-Embedder-Policy (COEP) header like this:

{: .quote }
> The HTTP Cross-Origin-Embedder-Policy (COEP) response header configures embedding cross-origin resources into the document.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Opener-Policy

A COEP header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the COEP header with a `require-corp` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseCrossOriginResourcePolicy()
    .UseCrossOriginEmbedderPolicy()
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
```

{: .warning }
> It is important to note that the recommended value for this header requires the presence of the
> [Cross-Origin-Resource-Policy (CORP) header](https://gaprogman.github.io/OwaspHeaders.Core/configuration/Cross-Origin-Resource-Policy/)
> in order to work.
> As such, if you add the COEP header without the CORP header, OwaspHeaders.Core will raise an ArgumentException.

The above adds the COEP header with a `require-corp` value.

## Full Options

The COEP header object (known internally as `CrossOriginEmbedderPolicy`) has the following options:

- enum: `CrossOriginEmbedderOptions`

The values available for the `CrossOriginEmbedderOptions` enum are:

- `UnsafeNoneValue`
- `RequireCorp`

