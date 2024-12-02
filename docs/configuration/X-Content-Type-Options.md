---
title: X-Content-Type-Options
nav_order: 4
parent: Configuration
---

The Mozilla Developer Network describes the X-Content-Type-Options header like this:

{: .quote }
> The X-Content-Type-Options response HTTP header is a marker used by the server to indicate that the MIME types advertised in the Content-Type headers should be followed and not be changed. The header allows you to avoid MIME type sniffing by saying that the MIME types are deliberately configured.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options

An X-Content-Type-Options header can be added in one of two ways, either using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

The above adds the X-Content-Type-Options header with a `nosniff` value.

Or by creating an instance of the `SecureHeadersMiddlewareBuilder` class using the following code:

```csharp
var customHstsConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentTypeOptions()
    .Build();

app.UseSecureHeadersMiddleware(customHstsConfig);
```

The above adds the X-Content-Type-Options header with a `nosniff` value.

{: .note }
The API for OwaspHeaders.Core does not support adding a value other than "nosniff" for the X-Content-Type-Options header.

## Full Options

There are no options for the X-Content-Type-Options header.

