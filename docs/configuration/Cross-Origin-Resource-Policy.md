---
title: Cross-Origin-Resource-Policy
nav_order: 7
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Cross-Origin-Resource-Policy (CORP) header like this:

{: .quote }
> The HTTP Cross-Origin-Resource-Policy response header indicates that the browser should block no-cors cross-origin or cross-site requests to the given resource.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Resource-Policy

A CORP header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the CORP header with a `same-origin` value.

Or by passing a configure delegate to `UseSecureHeadersMiddleware`, which is handed a `SecureHeadersBuilder`:

```csharp
app.UseSecureHeadersMiddleware(opt => opt.UseCrossOriginResourcePolicy());
```

The above adds the CORP header with a `same-origin` value.

## Full Options

The CORP header object (known internally as `CrossOriginResourcePolicy`) has the following options:

- enum: `CrossOriginResourceOptions`

The values available for the `CrossOriginResourceOptions` enum are:

- `CrossOrigin`
- `SameSite`
- `SameOrigin`

