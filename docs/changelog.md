---
title: Changelog
layout: page
nav_order: 9
---


This changelog represents all the major (i.e. breaking) changes made to the OwaspHeaders.Core project since it's inception. Early in the repo's development, GitHub's "releases" where used to release builds of the code repo. However shortly after it's inception, builds and releases where moved to [AppVeyor](https://ci.appveyor.com/project/GaProgMan/owaspheaders-core). Because of this, the releases on the GitHub repo became stale.

## TL;DR

| Major Version Number | Changes                                                                                                                                                                                                                                                                                                                                                                                                                     |
|----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 10                    | (as of Nov 12th, 2025) no API changes made yet. Library now supports ASP .NET Core (by updating the TFM to include `net10.0`) <br /> Added support for the EXPERIMENTAL Report-Endpoints header. This is listed nas EXPERIMENTAL (as of January 7th, 2025) on the [relevant MDN docs page](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Reporting-Endpoints), it is also not listed as a recommended header. As such, it is not added when the default builder is used |
| 9                    | Removed support for both .NET 6 and .NET 7 as these are no longer supported by Microsoft. It also adds support for .NET 9. <br /> A number of small optimisation have been made to the middleware's `Invoke` method <br /> Added support for both Cross-Origin-Opener-Policy (CORP) and Cross-Origin-Embedder-Policy (COEP) headers <br /> Added support for Clear-Site-Data header with path-specific configuration for logout scenarios <br/> Increased documentation coverage for Content-Security-Policy directive generation |
| 8                    | Removed support for ASP .NET Core on .NET Framework workflows; example and test projects now have OwaspHeaders.Core prefix, re-architected some of the test classes                                                                                                                                                                                                                                                         |
| 7                    | Added Cross-Origin-Resource-Policy header to list of defaults; simplified the use of the middleware in Composite Root/Program.cs                                                                                                                                                                                                                                                                                            |
| 6                    | Removes Expect-CT Header from the list of default headers                                                                                                                                                                                                                                                                                                                                                                   |
| 5                    | XSS Protection is now hard-coded to return "0" if enabled                                                                                                                                                                                                                                                                                                                                                                   |
| 4                    | Uses builder pattern to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> uses .NET Standard 2.0 <br /> Removed XSS Protection header from defaults                                                                                                                                                                                                                                                   |
| 3                    | Uses builder pattern to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Standard 2.0                                                                                                                                                                                                                                                                                                 |
| 2                    | Uses `secureHeaderSettings.json` and default config loader to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Core 2.0                                                                                                                                                                                                                                                               |
| 1                    | Uses `secureHeaderSettings.json` and default config loader to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Standard 1.4                                                                                                                                                                                                                                                           |

### Version 10

As of November 12th, 2025, no API changes have been added. This is a major version bump to maintain parity with the latest version of ASP .NET Core available. This version of the library still supports the following versions of ASP .NET Core:

- 8
- 9
- 10

As these are still in support by Microsoft.

#### Version 10.1.x

This version adds support for the experimental Reporting-Endpoints HTTP header, addressing issue #170. The Reporting-Endpoints header allows website administrators to specify endpoints where browsers can send violation reports from security policies like Content Security Policy (CSP). This header is marked as EXPERIMENTAL on MDN and is not included when using the default builder configuration.

**New Features:**

- New `ReportingEndpointsPolicy` model for configuring endpoint name-to-URI mappings
- `UseReportingEndpointsPolicy` builder method for middleware configuration
- Support for the modern `report-to` directive in Content Security Policy
- `RemoveTrailingCharacter` extension method for StringBuilder utility operations
- Comprehensive test coverage maintaining 65%+ code coverage requirement
- Full documentation and code examples for implementation

**Example Usage:**

```csharp
var reportingEndpoints = new Dictionary<string, Uri> {
    { "standard", new Uri("https://localhost:5000/reporting-endpoint") }
};
var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
    .UseReportingEndpointsPolicy(reportingEndpoints)
    .Build();
app.UseSecureHeadersMiddleware(config);
```

Note: The legacy `report-uri` directive continues to be supported but is marked as obsolete in favor of the modern `report-to` directive.

### Version 9

This version dropped support for .NET 6 and .NET 7, as they are no longer supported by Microsoft. It also added support for .NET 9.

All projects in the [GitHub repo](https://github.com/GaProgMan/OwaspHeaders.Core) now build and run with either .NET 8 or .NET 9, whichever is present (deferring to the highest version number if both are present). As of November 19th, 2024 there are no new features in Version 9, so if you still need to use the NuGet package with .NET 6 or 7 please use Version 8 of the package.

#### Version 9.9.x

This version adds support for the Clear-Site-Data HTTP header, addressing issue #32 and implementing the OWASP Secure Headers Project recommendation. The Clear-Site-Data header instructs browsers to clear client-side data (cache, cookies, storage) for specific paths, which is particularly important for logout endpoints to ensure complete session termination and prevent session hijacking.

**New Features:**

- Clear-Site-Data header support with path-specific configuration
- OWASP recommended default values (`"cache","cookies","storage"`)
- Support for all standard directives: "cache", "cookies", "storage", "executionContexts", and wildcard "*"
- Enum-based type safety with `ClearSiteDataOptions`
- Path-specific customization of which data types to clear
- Integration with existing middleware builder pattern
- Comprehensive documentation and examples for logout scenarios
- Full backward compatibility with existing configurations

See the [Clear-Site-Data documentation](https://gaprogman.github.io/OwaspHeaders.Core/configuration/Clear-Site-Data/) for detailed configuration options and examples.

#### Version 9.8.x

This version introduces comprehensive logging support via the `ILogger<SecureHeadersMiddleware>` interface, following Andrew Lock's high-performance logging best practices. The logging functionality provides visibility into middleware operations, helping developers troubleshoot configuration issues and monitor security header application.

**New Features:**

- Information-level logs for successful operations (middleware initialisation, headers added)
- Warning logs for configuration issues and header operation failures  
- Error logs for validation failures and middleware exceptions
- Debug logs for detailed header addition information
- Configurable Event IDs (1000-3999 range) to avoid conflicts with application logging
- High-performance logging with log level checking to minimise performance impact
- 100% backward compatibility - existing applications continue to work without changes

The logging is automatically enabled when an `ILogger<SecureHeadersMiddleware>` is available in dependency injection. Developers can customise Event IDs using `WithLoggingEventIdBase()` or `WithLoggingEventIds()` methods to avoid conflicts with existing application Event ID schemes.

See the [Logging documentation](https://gaprogman.github.io/OwaspHeaders.Core/logging) for detailed configuration options and examples.

#### Version 9.7.x

This version saw the addition of both the [Cross-Origin-Opener-Policy](https://gaprogman.github.io/OwaspHeaders.Core/configuration/Cross-Origin-Opener-Policy/) (COEP) and [Cross-Origin-Embedder-Policy](https://gaprogman.github.io/OwaspHeaders.Core/configuration/Cross-Origin-Embedder-Policy/) (COEP) headers; bringing the total number of supported headers to 83% complete (or 10 of the 12 recommended headers and values).

#### Version 9.6.x

This version saw the addition of a number of _very_ small changes to the middleware's `Invoke` method which aimed to increase efficiency, reduce working memory usage, and increase execution speed. 

#### Version 9.5.x

This version saw the addition of attestation generation on both a per PR-build and Release basis. See the [Attestations](https://gaprogman.github.io/OwaspHeaders.Core/attestations) page of the documentation to read about how you can verify the attestations per build or release.

#### Version 9.2.x

A number of small optimisations for generating HTTP header values have been made. There are also new Guard clauses in place to protect from a number of null or null/whitespace issues. All using statements have been cleaned up, with a large number placed in relevant global usings files.

**BREAKING CHANGE**: Removal of the X-Powered-By header has been completely removed in this version. The reason for this is that the X-Powered-By header is included by the reverse proxy, which ASP .NET Core has no control over. See the section in the Readme labelled "Server Header: A Warning" for details on how to remove this header.

#### Version 9.1.x

The `max-age` value for the Strict-Transport-Security (HSTS) header was updated to the OWASP recommended value of 31536000 (365 days).

### Version 8

This version dropped support for support for ASP .NET Core on .NET Framework workflows. This means that, from version 8 onwards, this package will no longer work with .NET Framework workloads. This decision was made as Microsoft have dropped support for ASP .NET Core on .NET Framework workloads. This can be seen in the ASP .NET Core support documentation [here](https://dotnet.microsoft.com/en-us/platform/support/policy/aspnet#dotnet-core)

> To help facilitate migrating applications to ASP.NET Core on .NET Core, the specified ASP.NET Core 2.1 packages (latest patched version only) will be supported on the .NET Framework and follow the support cycle for those .NET Framework versions. ASP.NET Core 2.1 is defined as "Tools" in the Microsoft Support Lifecycle Policy
> Source: https://dotnet.microsoft.com/en-us/platform/support/policy/aspnet#dotnet-core, obtained Oct 19th, 2024.

The Example and Tests csproj files (and directories) have been renamed to make the standard `OwaspHeaders.Core.{x}` where `{x}` is either `Example` (for the ASP .NET Core application which provides an example implementation) or `Tests` for the unit tests project.

#### Community Contributions

[swharden](https://github.com/swharden) created [PR #96](https://github.com/GaProgMan/OwaspHeaders.Core/pull/96) which greatly simplified and improved the NuGet package metadata and created a wonderful logo for the project.

---

### Version 7.5

This version makes it simpler to get started with the NuGet package by simplifying the use of it in Program.cs/Composite Root. This, effectively, changes the composite root from:

```csharp
app.UseSecureHeadersMiddleware(
    SecureHeadersMiddlewareExtensions
        .BuildDefaultConfiguration()
    );
```

to:

```csharp
app.UseSecureHeadersMiddleware();
```

---

### Version 7

This version adds the Cross-Origin-Resource-Policy header with the OWASP recommended value "same-origin" to the list of default headers in the `BuildDefaultConfiguration()` extension method. This was requested via [issue #76](https://github.com/GaProgMan/OwaspHeaders.Core/issues/76).

---

### Version 6

This version removes Expect-CT Header from the list of default headers in the `BuildDefaultConfiguration()` extension method. This is related to [issue #72](https://github.com/GaProgMan/OwaspHeaders.Core/issues/72).

All code which generates the header and it's value are still present, but it is removed from the defaults. Please see the above referenced issue for details.

---

### Version 5

This version of the repo ensure that the XSS Protection header (which was removed from the list of defaults in Version 4) is simplified down to the only recommended value (i.e. "0"), so that if a consumer enables XSS Protection they will only get the one possible value.

This is related to [guidance by MDN](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection) and by OWASP:

> Warning: The X-XSS-Protection header has been deprecated by modern browsers and its use can introduce additional security issues on the client side. As such, it is recommended to set the header as X-XSS-Protection: 0 in order to disable the XSS Auditor, and not allow it to take the default behavior of the browser handling the response. Please use Content-Security-Policy instead.

---

### Version 4

This version of the repo removed the XSS Protection Header from the list of default headers in the `BuildDefaultConfiguration()` extension method. This is related to [issue #44](https://github.com/GaProgMan/OwaspHeaders.Core/issues/44).

#### Version 4.0.1 Specific Changes

It was noticed that the `server:` header was not removed (this is defaulted to `server: kestrel` when running locally). Removal of this header was added when the `RemovePoweredByHeader` option (in the `SecureHeadersMiddlewareBuilder`) was chosen.

#### Version 4.2.0 Specific Changes

It was [requested](https://github.com/GaProgMan/OwaspHeaders.Core/issues/60) that a `Content-Security-Policy-Report-Only` header could be added. This header was added in version 4.0.2 of the library.

#### Version 4.4.0 Specific Changes

The beginnings of the Cache-Control header were added via a [PR](https://github.com/GaProgMan/OwaspHeaders.Core/pull/63). This PR was sent in via GitHub user [mkokabi](https://github.com/mkokabi).

#### Version 4.5.0 Specific Changes

It was pointed out that the Cache-Control header PR was incomplete, and caused production systems to begin caching responses which should not have been cached. This version brings in a configuration for the Cache-Control header which allows the user to add the following directives:

- `no-cache`
- `no-store`
- `must-revalidate`

These directives are added when building the configuration:

```c#
return SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .UseContentDefaultSecurityPolicy()
    .UsePermittedCrossDomainPolicies()
    .UseReferrerPolicy()
    .UseCacheControl() // <- this line
    .UseExpectCt(string.Empty, enforce: true)
    .RemovePoweredByHeader()
    .Build();
```

The above directives are added via `bool`s. For example, in order to add the `no-cache` header, you need to alter the call to `UseCacheControl()` to look like `UseCacheControl(noCache: true)`.

### Version 3

This version of the repo removed the dependency on .NET Core 2.0, and replaced it with a dependency on .NET Standard 2.0 (via the `netstandard2.0` Target Framework Moniker), and the version of the [Microsoft.AspNetCore.Http.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions/) was upped to version 2.1.1

This version of the repo was also changed to use the [Builder Pattern](https://en.wikipedia.org/wiki/Builder_pattern) for creating an instance of the `SecureHeadersMiddlewareConfiguration` class. This decision was taken because the configuration should be a build-time decision, rather than a runtime one. As such, a default builder was created for the `SecureHeadersMiddlewareConfiguration` class, which could be generated by calling the following:

``` csharp
app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration());
```

within the Startup class. This would call the following method:

``` csharp
public static SecureHeadersMiddlewareConfiguration BuildDefaultConfiguration()
{
    return SecureHeadersMiddlewareBuilder
        .CreateBuilder()
        .UseHsts()
        .UseXFrameOptions()
        .UseXSSProtection()
        .UseContentTypeOptions()
        .UseContentDefaultSecurityPolicy()
        .UsePermittedCrossDomainPolicies()
        .UseReferrerPolicy()
        .Build();
}
```

#### Version 3.0.0.0-2 Specific Changes

The `CspCommandType` enum was added, and consumed when creating Content Security Policy directives. This greatly simplifies the creation of Content Security policy directives, as the previous versions of the library used hard coded rules to decide whether a CSP rule was a directive (e.g. 'self') or a URL.

Also included where various changes in the placement of semi-colon characters (i.e. ';') within the hard coded string values for certain headers. These are replaced with a single semi-colon which is appended to the header at the very end of rendering it's value.

#### Version 3.0.0.3 Specific Changes

This version fixed casing based typos to the `XFrameOptions` enum. It also fixed case based typos in the rendered values for the X-Frame-Options header value (i.e. `sameorigin` was replaced with `SOMEORIGIN`)

#### Version 3.1.0.0 Specific Changes

Added support for removing the `X-Powered-By` header value. This required the creation of an extension method called `TryRemoveHeader`, which would return true/false based on whether the header was present and could be removed. This was also added to the default configuration by adding a call to `RemovePoweredByHeader()` to the default config builder.

#### Version 3.1.0.1 Specific Changes

This version fixed a typo-based issue which was created during the 3.0.0.3 changes: namely `XFrameOptions.Sameorigin` was rendering as `SOMEORIGIN`. This was changed to `SAMEORIGIN` in this version.

#### Version 3.1.1 Specific Changes

An SVG logo was added to the project, ready to be consumed in the NuSpec file.

#### Version 3.2.0 Specific Changes

Support for the `Expect-CT` header was added - as per [this issue](https://github.com/GaProgMan/OwaspHeaders.Core/issues/19). The addition of this header was exposed via an extension method called `UseExpectCt()` which can be called when building the configuration object.

#### Version 3.2.2 Specific Changes

Removed the explicit dependency on the Microsoft.AspNetcore.All metapackage - as per [this issue](https://github.com/GaProgMan/OwaspHeaders.Core/issues/28). This change required taking a direct dependency on the Microsoft.AspNetCore.Http.Abstractions package (which is what the metapackage was supplying).

---

### Version 2

This version of the repo continued to use the `secureHeaderSettings.json`, which was consumed in the Startup class. However, this version of the repo has an explicit dependency on .NET Core 2.0 (it uses the `netcoreapp2.0` Target Framework Moniker), and [Microsoft.AspNetCore.Http.Abstractions version 2.0.0](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions/2.0.0).

### Version 1

The initial version of the repo relied on the presence of .NET Standard 1.0 and [Microsoft.AspNetCore.Http.Abstractions version 1.1.1](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions/1.1.1).

The initial version of the repo used a file called `secureHeaderSettings.json` which took the following format:

``` json
{
    "SecureHeadersMiddlewareConfiguration": {
        "UseHsts": "true",
        "HstsConfiguration": {
            "MaxAge": 42,
            "IncludeSubDomains": "true"
        },
        "UseHpkp": "true",
        "HPKPConfiguration" :{
            "PinSha256" : [
                "e927fad33f9eb96126896413502a1034be0ca379dec377fb891feb9ebc720e47"
                ],
            "MaxAge": 3,
            "IncludeSubDomains": "true",
            "ReportUri": "https://github.com/GaProgMan/OwaspHeaders.Core"
        },
        "UseXFrameOptions": "true",
        "XFrameOptionsConfiguration": {
            "OptionValue": "allowfrom",
            "AllowFromDomain": "com.gaprogman.dotnetcore"
        },
        "UseXssProtection": "true",
        "XssConfiguration": {
            "XssSetting": "oneReport",
            "ReportUri": "https://github.com/GaProgMan/OwaspHeaders.Core"
        },
        "UseXContentTypeOptions": "true",
        "UseContentSecurityPolicy": "true",
        "ContentSecurityPolicyConfiguration": {
            "BlockAllMixedContent": "true",
            "UpgradeInsecureRequests": "true"
        }
    }
}
```

This was read and used to construct an instance of the `SecureHeadersMiddlewareConfiguration` class (which represented the values used to render the secure headers) at runtime.

The json file containing the configuration was loaded in the Startup class in the following manner:

``` csharp
public Startup(IHostingEnvironment env)
{
    var builder = new ConfigurationBuilder()
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddJsonFile("secureHeaderSettings.json", optional:true, reloadOnChange: true)
    .AddEnvironmentVariables();
    Configuration = builder.Build();
}
```

As the code used the default configuration builder, changing the contents of the `secureHeaderSettings.json` would force the consuming application to reboot. This would mean that requests which were being dealt with _could_ be destroyed, and responses never generated for them.