# Changelog

This changelog represents all of the major (i.e. breaking) changes made to the OwaspHeaders.Core project since it's inception. Early in the repo's development, GitHub's "releases" where used to release builds of the code repo. However shortly after it's inception, builds and releases where moved to [AppVeyor](https://ci.appveyor.com/project/GaProgMan/owaspheaders-core). Because of this, the releases on the GitHub repo became stale.

## TL;DR

| Major Version Number | Changes |
|---|---|
| 5 | XSS Protection is now hard-coded to return "0" if enabled |
| 4 | Uses builder pattern to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> uses .NET Standard 2.0 <br /> Removed XSS Protection header from defaults |
| 3 | Uses builder pattern to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Standard 2.0 |
| 2 | Uses `secureHeaderSettings.json` and default config loader to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Core 2.0 |
| 1 | Uses `secureHeaderSettings.json` and default config loader to create instances of `SecureHeadersMiddlewareConfiguration` class <br /> also uses .NET Standard 1.4 |

### Version 6

This version removes Expect-CT Header from the list of default headers in the `BuildDefaultConfiguration()` extension method. This is related to [issue #72](https://github.com/GaProgMan/OwaspHeaders.Core/issues/72).

All code which generates the header and it's value are still present, but it is removed from the defaults. Please see the above referenced issue for details.

### Version 5

This version of the repo ensure that the XSS Protection header (which was removed from the list of defaults in Version 4) is simplified down to the only recommended value (i.e. "0"), so that if a consumer enables XSS Protection they will only get the one possible value.

This is related to [guidance by MDN](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection) and by OWASP:

> Warning: The X-XSS-Protection header has been deprecated by modern browsers and its use can introduce additional security issues on the client side. As such, it is recommended to set the header as X-XSS-Protection: 0 in order to disable the XSS Auditor, and not allow it to take the default behavior of the browser handling the response. Please use Content-Security-Policy instead.

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