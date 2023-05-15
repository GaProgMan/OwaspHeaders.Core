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

3. Alter the Startup (pre .NET 6) or program (post .NET 6) class to include the following:

```csharp
app.UseSecureHeadersMiddleware(
    SecureHeadersMiddlewareExtensions
        .BuildDefaultConfiguration()
    );
```

This will add a number of default HTTP headers to all responses from your server component.

The following is an example of the response headers from version 6.0.2 (taken on May 15th, 2023)

``` plaintext
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

## Source Code Repo

The source code for this NuGet package can be found at: [https://github.com/GaProgMan/OwaspHeaders.Core](https://github.com/GaProgMan/OwaspHeaders.Core).

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
