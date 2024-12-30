# OwaspHeaders.Core

An ASP .NET Core middleware for injection [OWASP](https://www.owasp.org/index.php/Main_Page) recommended HTTP Headers for increased security. This project is designed against the [OWASP Secure Headers Project](https://owasp.org/www-project-secure-headers/).

## Quick Starts

1. Create a .NET (either Framework, Core, or 5+) project which uses ASP .NET Core

Example;

```bash
dotnet new webapi -n exampleProject
```

2. Add a reference to the OwaspHeaders.Core NuGet package.

Example:

```bash
dotnet add package OwaspHeaders.Core
```

3. Alter the program.cs file to include the following:

```csharp
app.UseSecureHeadersMiddleware();
```

This will add a number of default HTTP headers to all responses from your server component.

The following is an example of the response headers from version 9.0.0 (taken on November 19th, 2024)

```http
cache-control: max-age=31536000,private 
content-security-policy: script-src 'self';object-src 'self';block-all-mixed-content;upgrade-insecure-requests; 
cross-origin-resource-policy: same-origin 
referrer-policy: no-referrer 
strict-transport-security: max-age=31536000;includeSubDomains 
x-content-type-options: nosniff 
x-frame-options: DENY 
x-permitted-cross-domain-policies: none; 
x-xss-protection: 0 
```

Please note: The above example contains only the headers added by the Middleware.

## Source Code Repo

The source code for this NuGet package can be found at: [https://github.com/GaProgMan/OwaspHeaders.Core](https://github.com/GaProgMan/OwaspHeaders.Core).

## Documentation

The documentation for this NuGet package can be found at: [https://gaprogman.github.io/OwaspHeaders.Core/](https://gaprogman.github.io/OwaspHeaders.Core/).

### Attestations

As of [PR 148](https://github.com/GaProgMan/OwaspHeaders.Core/pull/148), OwaspHeaders.Core uses the GitHub provided process for creating attestations per build. This document talks through how to verify those attestations using the [gh CLI](https://cli.github.com/).

See the [Attestations](https://gaprogman.github.io/OwaspHeaders.Core/attestations) page of the documentation to read about how you can verify the attestations for builds from 9.5.0 onward.

## Issues and Bugs

Please raise any issues and bugs at the above mentioned source code repo.

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
