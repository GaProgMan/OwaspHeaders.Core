# OwaspHeaders.Core

An ASP.NET Core middleware designed to increase web application security by adopting the [OWASP Secure Headers project](https://www.owasp.org/index.php/OWASP_Secure_Headers_Project) recommended HTTP headers and values.

| Build Status | Release Status | License used  | OpenSSF |
| -------------|----------------|--------------|---------|
| [![Build status](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/dotnet.yml) | [![Release](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/release.yml/badge.svg)](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/release.yml) | [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) | [![OpenSSF Best Practices](https://www.bestpractices.dev/projects/9723/badge)](https://www.bestpractices.dev/projects/9723) |

Please note: this middleware **DOES NOT SUPPORT BLAZOR OR WEBASSEMBLY APPLICATIONS**. This is because setting up secure HTTP headers in a WebAssembly context is a non-trivial task.

## Tools Required to Build This Repo

- .NET SDKs vLatest
  - 8.0
  - 9.0
- an IDE (VS Code, Rider, or Visual Studio)
- [dotnet-format](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format) global tool.

That's it.

## Example Project Coding Guidelines

### Primary Constructors Restriction

**Important**: When contributing to the **example project only** (`OwaspHeaders.Core.Example` directory), please avoid using primary constructors due to a known issue with `dotnet-format` that causes incorrect indentation.

#### ❌ Don't use (in example project):

```csharp
public class HomeController(ILogger<HomeController> logger) : ControllerBase
{
    private readonly ILogger<HomeController> _logger = logger;
    // dotnet-format will incorrectly indent methods here
}
```

#### ✅ Use instead (in example project):

```csharp
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    // dotnet-format handles this correctly
}
```

**Why**: This restriction exists because of a bug in `dotnet-format` when processing primary constructors (see [dotnet/format#2165](https://github.com/dotnet/format/issues/2165)). Since this project uses `.editorconfig` and `dotnet-format` for consistent code formatting, primary constructors cause formatting issues that break our CI/CD pipeline.

**Scope**: This restriction applies **only to the example project**. The main OwaspHeaders.Core library does not use primary constructors and is not affected by this issue.

**Future**: This guidance will be removed once the upstream `dotnet-format` bug is resolved.

## Documentation

The latest documentation for OwaspHeaders.Core can be found at [https://gaprogman.github.io/OwaspHeaders.Core/](https://gaprogman.github.io/OwaspHeaders.Core/).

### Attestations

As of [PR 148](https://github.com/GaProgMan/OwaspHeaders.Core/pull/148), OwaspHeaders.Core uses the GitHub provided process for creating attestations per build. This document talks through how to verify those attestations using the [gh CLI](https://cli.github.com/).

See the [Attestations](https://gaprogman.github.io/OwaspHeaders.Core/attestations) page of the documentation to read about how you can verify the attestations for builds from 9.5.0 onward.

## Pull Requests

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://github.com/GaProgMan/OwaspHeaders.Core/pulls)

Pull requests are welcome, but please take a moment to read the [Code of Conduct](https://github.com/GaProgMan/OwaspHeaders.Core?tab=coc-ov-file) before submitting them or commenting on any work in this repo.

We have comprehensive documentation for contributing to this project which you are encouraged to reach. This documentation can be found at: [https://gaprogman.github.io/OwaspHeaders.Core/Contributing/](https://gaprogman.github.io/OwaspHeaders.Core/Contributing/).

## Getting Started

Assuming that you have an ASP .NET Core project, add the NuGet package:

```bash
dotnet add package OwaspHeaders.Core
```

Alter the program.cs file to include the following:

```csharp
app.UseSecureHeadersMiddleware();
```

This will add a number of default HTTP headers to all responses from your server component.

The following is an example of the response headers from version 9.0.0 (taken on November 19th, 2024)

```http
strict-transport-security: max-age=31536000;includesubdomains
x-frame-options: deny
x-content-type-options: nosniff
content-security-policy: script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;
x-permitted-cross-domain-policies: none
referrer-policy: no-referrer
cross-origin-resource-policy: same-origin
cache-control: max-age=0,no-store
cross-origin-opener-policy: same-origin
cross-origin-embedder-policy: require-corp
x-xss-protection: 0
```

Please note: The above example contains only the headers added by the Middleware.

### Secure Headers

The `SecureHeadersMiddleware` is used to inject the HTTP headers recommended by the [OWASP Secure Headers](https://www.owasp.org/index.php/OWASP_Secure_Headers_Project) project into all responses generated by the ASP.NET Core pipeline.

Listing and commenting on the default values that this middleware provides is out of scope for this readme&mdash;but can be found in [the official documentation](https://gaprogman.github.io/OwaspHeaders.Core)&mdash;. Please note that you will need to read through the above link to the Secure Headers Project in order to understand what these headers do, and the affect their presence will have on your applications when running in a web browser.

## Configuration

This Middleware uses the builder pattern to set up the header information, which is a compile time dependency.

In your `Program.cs` file:

https://github.com/GaProgMan/OwaspHeaders.Core/blob/433cbb764956e86b80b598c5d0760bdfdef28161/example/Program.cs#L26

This will use the default configuration for the OwaspHeaders.Core middleware. The method (found in `/src/Extensions/SecureHeadersMiddlewareExtensions.cs`) looks like this:

https://github.com/GaProgMan/OwaspHeaders.Core/blob/433cbb764956e86b80b598c5d0760bdfdef28161/src/Extensions/SecureHeadersMiddlewareExtensions.cs#L23-L38

### Custom Configuration

In order to use a custom configuration, follow the same pattern (perhaps creating your own extension method to encapsulate it):

``` csharp
public static SecureHeadersMiddlewareConfiguration CustomConfiguration()
{
    return SecureHeadersMiddlewareBuilder
        .CreateBuilder()
        .UseHsts(1200, false)
        .UseContentDefaultSecurityPolicy()
        .UsePermittedCrossDomainPolicy
            (XPermittedCrossDomainOptionValue.masterOnly)
        .UseReferrerPolicy(ReferrerPolicyOptions.sameOrigin)
        .Build();
}
```

Then consume it in the following manner:

```csharp
app.UseSecureHeadersMiddleware(
    CustomSecureHeaderExtensions.CustomConfiguration()
);
```

#### Testing the Middleware

An example ASP .NET Core application - with the middleware installed -  is provided as part of this repo (see the code in the `OwaspHeaders.Core.Example` directory). As such, you can run this example application to see the middleware in use via a provided OpenAPI endpoint - located at `/swagger`.

Or you could add the middleware to an existing application and run through the following Run the application, request one of the pages that it serves and view the headers for the page.

This can be done in Google Chrome, using the Dev tools and checking the network tab.

![secure headers shown in network tab](screenshots/secure-headers-screenshot.png "Headers on the right-hand side here")

Shown above in the `Response Headers` section of the `Values` response.

## Server Header: A Warning

The default configuration for this middleware removes the `X-Powered-By` header, as this can help malicious users to use targeted attacks for specific server infrastructure. However, since the `Server` header is added by the reverse proxy used when hosting an ASP .NET Core application, removing this header is out of scope for this middleware.

In order to remove this header, a `web.config` file is required, and the following should be added to it:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <security>
            <requestFiltering removeServerHeader="true" />
        </security>
    </system.webServer>
</configuration>
```

The above XML is taken from [this answer on ServerFault](https://serverfault.com/a/1020784).

The `web.config` file will need to be copied to the server when the application is deployed.
