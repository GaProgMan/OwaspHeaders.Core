---
name: owaspheaders-core
description: Rules for adding, changing or reviewing HTTP security headers in an ASP.NET Core app that uses the OwaspHeaders.Core NuGet package. Use when editing Program.cs or middleware that touches UseSecureHeadersMiddleware, Content-Security-Policy (CSP), Strict-Transport-Security (HSTS), X-Frame-Options, Referrer-Policy, Cache-Control, Cross-Origin-* or Clear-Site-Data headers, when a page is blocked by CSP, or when upgrading OwaspHeaders.Core code from version 10 or earlier.
license: MIT
---

# OwaspHeaders.Core

This guidance is for **OwaspHeaders.Core 11.x**, the version installed in this repository. It was installed by the package itself and is replaced whenever the package is updated. Code you remember from older versions of this package, or from blog posts, may no longer compile or may be deprecated.

## 1. Never write these headers by hand

This middleware owns the following response headers. Do not set them with `Response.Headers`, `app.Use(...)`, a custom middleware, a filter, or `app.UseHsts()`:

`Strict-Transport-Security`, `X-Frame-Options`, `X-Content-Type-Options`, `Content-Security-Policy`, `Content-Security-Policy-Report-Only`, `X-Content-Security-Policy`, `X-Permitted-Cross-Domain-Policies`, `Referrer-Policy`, `Cache-Control`, `X-XSS-Protection`, `Cross-Origin-Resource-Policy`, `Cross-Origin-Opener-Policy`, `Cross-Origin-Embedder-Policy`, `Clear-Site-Data`, `Reporting-Endpoints`.

If a header is already on the response when the middleware runs, the middleware leaves it alone and says nothing. So a hand-written value silently replaces the configured one. **Do not write this:**

<!-- sample: hand-rolled-headers -->
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});
```

Configure the header through the middleware instead (section 2). If the builder cannot express what is needed, stop and tell the user rather than working around it.

## 2. The version 11 API

`UseSecureHeadersMiddleware` lives in the `OwaspHeaders.Core.Extensions` namespace. The recommended OWASP header set is one line:

<!-- sample: recommended-defaults -->
```csharp
app.UseSecureHeadersMiddleware();
```

To change anything, pass a configure delegate. A new builder starts empty, so call `UseRecommendedDefaults()` first and adjust from there:

<!-- sample: configure-delegate -->
```csharp
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseRecommendedDefaults();
    opt.UseHsts(maxAge: 63072000);
    opt.SetUrlsToIgnore(["/health"]);
});
```

The configuration is validated while the pipeline is built, so a mistake stops the application from starting. To check the same configuration from a test without a host, give the delegate a name and pass it to `SecureHeadersBuilder.BuildAndValidate`, which throws on an invalid configuration:

<!-- sample: shared-configuration -->
```csharp
static void ConfigureSecureHeaders(SecureHeadersBuilder opt)
{
    opt.UseRecommendedDefaults();
    opt.SetUrlsToIgnore(["/health"]);
}

app.UseSecureHeadersMiddleware(ConfigureSecureHeaders);

// In a test:
SecureHeadersBuilder.BuildAndValidate(ConfigureSecureHeaders);
```

Code from version 10 or earlier needs changing:

| Old code | Status in 11.x | Write instead |
|---|---|---|
| `SecureHeadersMiddlewareBuilder.CreateBuilder()...Build()` | `[Obsolete]`, removed in 12 | The configure delegate above |
| `SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration()` | `[Obsolete]`, removed in 12 | `app.UseSecureHeadersMiddleware()` |
| `app.UseSecureHeadersMiddleware(config, urlIgnoreList)` | `[Obsolete]`, removed in 12 | The configure delegate, with `opt.SetUrlsToIgnore(...)` |
| Assigning properties on `SecureHeadersMiddlewareConfiguration`, e.g. `config.UseHsts = true` | Does not compile (setters are `internal`) | The matching `opt.UseX(...)` builder method |
| `.UseExpectCt(...)` or `ExpectCt` | Removed | Delete it; Expect-CT is obsolete and has no replacement |
| `UseContentSecurityPolicyReportOnly(...)` | `[Obsolete]` | `opt.UseContentSecurityPolicyReportUriOnly(...)` |

Do not silence `CS0618` for these members. The warning is how the package points callers at the replacement.

## 3. Where it goes in the pipeline

The middleware adds its headers before calling the next middleware, so anything registered before it that writes a response (static files, redirects, endpoints) sends that response without the headers. Register it straight after exception handling and before everything else:

<!-- sample: pipeline-placement -->
```csharp
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSecureHeadersMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
```

## 4. Content-Security-Policy

When a browser blocks a script, style, image or frame because of the CSP, **do not add `'unsafe-inline'` or `'unsafe-eval'`**, and do not remove or loosen the policy to make the error go away. That brings back the attacks the header exists to stop. Instead:

- Allow the specific origin the resource comes from, as below.
- Move inline scripts and styles into files served from the application's own origin.
- If neither is possible, ask the user how they want to proceed.

`SetCspUris` needs `using OwaspHeaders.Core.Enums;` and `using OwaspHeaders.Core.Models;`. It **replaces** the source list for the directive, so restate `'self'` when you add an origin:

<!-- sample: csp-allow-origin -->
```csharp
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseRecommendedDefaults();
    opt.SetCspUris(
    [
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdn.example.com" }
    ], CspUriType.Script);
});
```

The sanctioned escape hatch for a route that genuinely cannot work with the headers is `opt.SetUrlsToIgnore([...])`. It skips **every** header for those paths, not just the CSP, and matches each path exactly and case-sensitively (there is no prefix or wildcard matching). Use it for a few named paths only, and say which paths you excluded and why.

## 5. What this package does not do

- **Blazor and WebAssembly applications are not supported.** Do not add this middleware to a Blazor WebAssembly project, and do not use it to set headers for one.
- **The `Server` header is out of scope.** It is added by the web server or reverse proxy, not by ASP.NET Core middleware. Under IIS, remove it with `<requestFiltering removeServerHeader="true" />` in `web.config`. Under Kestrel, set `KestrelServerOptions.AddServerHeader = false`. Do not try to remove it with this middleware.
