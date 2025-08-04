---
title: Content-Security-Policy (CSP)
nav_order: 4
parent: Configuration
layout: page
---

The Mozilla Developer Network describes the Content-Security-Policy header as follows:

{: .quote }
> Content Security Policy (CSP) is an added layer of security that helps to prevent and mitigate certain types of attacks, including Cross Site Scripting (XSS) and data injection attacks. These attacks are used for everything from data theft to site defacement to distribution of malware.
>
> source: https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP

Content Security Policy is a powerful security feature but requires careful configuration. It works by defining approved sources of content that browsers should trust when loading resources.

{: .note }
CSP can break functionality if not configured properly. It's recommended to start with `Content-Security-Policy-Report-Only` mode to test your policy before enforcing it.

## Basic Configuration

### Default Secure Configuration

The simplest way to add a secure CSP header is using the default middleware options:

```csharp
app.UseSecureHeadersMiddleware();
```

This includes a basic CSP configuration with the following directives:

| Directive | Value |
|-----------|--------|
| script-src | 'self' |
| object-src | 'self' |
| block-all-mixed-content | (enabled) |
| upgrade-insecure-requests | (enabled) |

### Default CSP Configuration Method

For more explicit control over CSP defaults, use:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseDefaultContentSecurityPolicy()
    .Build();

app.UseSecureHeadersMiddleware(config);
```

This produces the same CSP header as above but makes the CSP configuration explicit in your code.

### Custom CSP Configuration

For custom CSP policies, create your own configuration:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicy(
        pluginTypes: null,
        blockAllMixedContent: true,
        upgradeInsecureRequests: true,
        reportUri: "https://example.com/csp-report")
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective(),
        new ContentSecurityPolicyElement
        {
            CommandType = CspCommandType.Uri,
            DirectiveOrUri = "https://cdn.example.com"
        }
    ], CspUriType.Script)
    .Build();

app.UseSecureHeadersMiddleware(config);
```

The above configuration allows scripts from the same origin and a specific CDN, while reporting violations to a specified URI.

## Configuration Methods

### UseContentSecurityPolicy

The main CSP configuration method with full customisation options:

```csharp
.UseContentSecurityPolicy(
    pluginTypes: "application/pdf",
    blockAllMixedContent: true,
    upgradeInsecureRequests: true,
    referrer: "no-referrer",
    reportUri: "https://example.com/csp-report")
```

**Parameters:**

- `pluginTypes` - MIME types allowed for plugins (deprecated in most browsers)
- `blockAllMixedContent` - Prevents loading HTTP resources on HTTPS pages
- `upgradeInsecureRequests` - Automatically upgrades HTTP requests to HTTPS
- `referrer` - Controls referrer information sent with requests (deprecated, use Referrer-Policy header instead)
- `reportUri` - URI to send violation reports to

### UseContentSecurityPolicyReportOnly

For testing CSP policies without enforcing them:

```csharp
.UseContentSecurityPolicyReportOnly(
    reportUri: "https://example.com/csp-report",
    blockAllMixedContent: true,
    upgradeInsecureRequests: false)
```

This sends the `Content-Security-Policy-Report-Only` header instead of `Content-Security-Policy`, allowing you to test policies without breaking functionality.

## Content Security Policy Directives

CSP uses directives to control which resources can be loaded. The middleware supports all major CSP directives:

### Source Directives

These directives control where specific types of resources can be loaded from:

| Directive | Purpose | CspUriType |
|-----------|---------|------------|
| default-src | Fallback for other source directives | DefaultUri |
| script-src | JavaScript sources | Script |
| style-src | CSS sources | Style |
| img-src | Image sources | Img |
| font-src | Font sources | Font |
| connect-src | XMLHttpRequest, WebSocket, EventSource connections | Connect |
| media-src | Audio and video sources | Media |
| object-src | Object, embed, applet sources | Object |
| child-src | Web workers and nested browsing contexts | Child |
| frame-src | Frame and iframe sources | Frame |
| manifest-src | Web app manifest sources | Manifest |

### Navigation Directives

| Directive | Purpose | CspUriType |
|-----------|---------|------------|
| form-action | Form submission targets | Form |
| frame-ancestors | Valid parents for embedding (replaces X-Frame-Options) | FrameAncestors |
| base-uri | Valid URIs for document base element | Base |

### Example: Configuring Multiple Directives

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicy()
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective()
    ], CspUriType.DefaultUri)
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective(),
        new ContentSecurityPolicyElement
        {
            CommandType = CspCommandType.Uri,
            DirectiveOrUri = "https://cdn.jsdelivr.net"
        }
    ], CspUriType.Script)
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective(),
        new ContentSecurityPolicyElement
        {
            CommandType = CspCommandType.Directive,
            DirectiveOrUri = "unsafe-inline"
        }
    ], CspUriType.Style)
    .Build();
```

This configuration:

- Sets `default-src 'self'` as the fallback
- Allows scripts from same origin and jsDelivr CDN
- Allows styles from same origin and inline styles

## Content Security Policy Elements

CSP directives are built using `ContentSecurityPolicyElement` objects with two types:

### Directive Elements

Common CSP keywords that must be quoted in the final header:

```csharp
new ContentSecurityPolicyElement
{
    CommandType = CspCommandType.Directive,
    DirectiveOrUri = "self"        // Becomes 'self'
}

new ContentSecurityPolicyElement
{
    CommandType = CspCommandType.Directive,
    DirectiveOrUri = "unsafe-inline"  // Becomes 'unsafe-inline'
}
```

**Common Directive Values:**

- `self` - Same origin as the document
- `unsafe-inline` - Allow inline scripts/styles (not recommended)
- `unsafe-eval` - Allow eval() and similar methods (not recommended)
- `none` - Block all sources of this type
- `strict-dynamic` - Allow scripts loaded by trusted scripts
- `unsafe-hashes` - Allow specific inline event handlers

### URI Elements

Specific domains, protocols, or paths:

```csharp
new ContentSecurityPolicyElement
{
    CommandType = CspCommandType.Uri,
    DirectiveOrUri = "https://example.com"
}

new ContentSecurityPolicyElement
{
    CommandType = CspCommandType.Uri,
    DirectiveOrUri = "*.googleapis.com"
}
```

### Helper Methods

The middleware provides helper methods for common scenarios:

```csharp
// Create 'self' directive
ContentSecurityPolicyHelpers.CreateSelfDirective()
```

## Sandbox Policies

CSP can apply sandbox restrictions similar to iframe sandboxing:

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicy()
    .SetCspSandBox(
        CspSandboxType.allowForms,
        CspSandboxType.allowScripts,
        CspSandboxType.allowSameOrigin)
    .Build();
```

### Available Sandbox Types

| CspSandboxType | Permission |
|----------------|------------|
| allowForms | Allow form submission |
| allowModals | Allow modal dialogs |
| allowOrientationLock | Allow screen orientation lock |
| allowPointerLock | Allow pointer lock |
| allowPopups | Allow popups |
| allowPopupsToEscapeSandbox | Allow popups to escape sandbox |
| allowPresentation | Allow presentation mode |
| allowSameOrigin | Allow same-origin access |
| allowScripts | Allow script execution |
| allowTopNavigation | Allow top-level navigation |

{: .note }
Empty sandbox directive (no permissions) applies the most restrictive sandbox policy.

## Practical Examples

### Basic Secure Website

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicy(blockAllMixedContent: true, upgradeInsecureRequests: true)
    .SetCspUris([ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.DefaultUri)
    .SetCspUris([
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "none" }
    ], CspUriType.Object)
    .Build();
```

Results in: `default-src 'self'; object-src 'none'; block-all-mixed-content; upgrade-insecure-requests;`

### Website Using CDNs

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicy()
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective(),
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdn.jsdelivr.net" },
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdnjs.cloudflare.com" }
    ], CspUriType.Script)
    .SetCspUris([
        ContentSecurityPolicyHelpers.CreateSelfDirective(),
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://fonts.googleapis.com" }
    ], CspUriType.Style)
    .SetCspUris([
        new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://fonts.gstatic.com" }
    ], CspUriType.Font)
    .Build();
```

### Report-Only Testing Configuration

```csharp
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseContentSecurityPolicyReportOnly(
        reportUri: "https://example.com/csp-report",
        blockAllMixedContent: false,
        upgradeInsecureRequests: false)
    .SetCspUris([ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.DefaultUri)
    .Build();
```

This sends violations to your reporting endpoint without blocking resources, allowing you to test policies safely.

## Full Options

The CSP configuration objects have the following options:

### ContentSecurityPolicyConfiguration

**Constructor Parameters:**

- `string pluginTypes` - MIME types for plugins
- `bool blockAllMixedContent` - Block mixed content
- `bool upgradeInsecureRequests` - Upgrade HTTP to HTTPS
- `string referrer` - Referrer policy (deprecated)
- `string reportUri` - Violation reporting URI

**Properties (`List<ContentSecurityPolicyElement>`):**

- `BaseUri` - base-uri directive sources
- `DefaultSrc` - default-src directive sources  
- `ScriptSrc` - script-src directive sources
- `ObjectSrc` - object-src directive sources
- `StyleSrc` - style-src directive sources
- `ImgSrc` - img-src directive sources
- `MediaSrc` - media-src directive sources
- `FrameSrc` - frame-src directive sources
- `ChildSrc` - child-src directive sources
- `FrameAncestors` - frame-ancestors directive sources
- `FontSrc` - font-src directive sources
- `ConnectSrc` - connect-src directive sources
- `ManifestSrc` - manifest-src directive sources
- `FormAction` - form-action directive sources

**Sandbox Property:**

- `ContentSecurityPolicySandBox Sandbox` - Sandbox policy configuration

### Extension Methods

- `SetCspUris(List<ContentSecurityPolicyElement>, CspUriType)` - Configure directive sources
- `SetCspSandBox(params CspSandboxType[])` - Configure sandbox permissions

These methods can be called when creating a custom configuration to set up specific CSP directives and sandbox policies.
