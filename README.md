# OwaspHeaders.Core

A collection of ASP.NET Core middleware classes designed to increase web application security by adopting the recommended [OWASP](https://www.owasp.org/index.php/Main_Page) settings.

| Build Status | Release Status | License used | Changelog | Code of Conduct |
| -------------|----------------|-----------|--------------|-----------------|
| [![Build status](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/dotnet.yml) | [![Release](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/release.yml/badge.svg)](https://github.com/GaProgMan/OwaspHeaders.Core/actions/workflows/release.yml) | [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) | [changelog](changelog.md) | [Code of Conduct.md](Code-of-Conduct.md) |

Please note: this middleware **DOES NOT SUPPORT BLAZOR OR WEBASSEMBLY APPLICATIONS**. This is because setting up secure HTTP headers in a WebAssembly context is a non-trivial task.

## Tools Required to Build This Repo

- .NET SDKs vLatest
  - 6.0
  - 7.0
  - 8.0
- an IDE (VS Code, Rider, or Visual Studio)
- [dotnet-format](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format) global tool.

That's it.

## Pull Requests

[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

Pull requests are welcome, but please take a moment to read the Code of Conduct before submitting them or commenting on any work in this repo.

Also please make sure to run `dotnet format OwaspHeaders.Core.sln` in the root of the repo _before_ submitting a PR. This repo uses an [editorconfig](.editorconfig) file to enforce certain formatting rules on this repo. Any PRs which don't adhere to these formatting rules will fail a PR action (for checking the code against the rules). So to save time, please run `dotnet format OwaspHeaders.Core.sln` ahead of submitting your PR.

## Getting Started

Assuming that you have an ASP .NET Core project, add the NuGet package:

```bash
dotnet add package OwaspHeaders.Core
```

Alter the Startup (pre .NET 6) or program (post .NET 6) class to include the following:

```csharp
app.UseSecureHeadersMiddleware();
```

This will add a number of default HTTP headers to all responses from your server component.

The following is an example of the response headers from version 6.0.2 (taken on May 15th, 2023)

```plaintext
cache-control: max-age=31536000, private
strict-transport-security: max-age=63072000;includeSubDomains
x-frame-options: DENY
x-xss-protection: 0
x-content-type-options: nosniff
content-security-policy: script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests;
x-permitted-cross-domain-policies: none;
referrer-policy: no-referrer
```

Please note: The above example contains only the headers added by the Middleware.

### Secure Headers

The `SecureHeadersMiddleware` is used to inject the HTTP headers recommended by the [OWASP Secure Headers](https://www.owasp.org/index.php/OWASP_Secure_Headers_Project) project into all responses generated by the ASP.NET Core pipeline.

Listing and commenting on the default values that this middleware provides is out of scope for this readme. Please note that you will need to read through the above link to the Secure Headers Project in order to understand what these headers do, and the affect their presence will have on your applications when running in a web browser.

## Configuration

This Middleware uses the builder pattern to set up the header information, which is a compile time dependency.

In your `Startup` class (or `Program.cs` for .NET 6 onwards):

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
