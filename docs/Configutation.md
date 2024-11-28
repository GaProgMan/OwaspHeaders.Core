---
title: Configuration
layout: page
nav_order: 2
---

This Middleware uses the builder pattern to set up the header information, which is a compile time dependency. If you need to change the configuration, you will need to rebuild your application.

## Basic Configuration

Adding the default configuration by adding the following to your `Program.cs` somewhere in the Middleware pipeline:

```csharp
app.UseSecureHeadersMiddleware();
```

This will use the default configuration for the OwaspHeaders.Core middleware. The method looks like this:

```csharp
public static SecureHeadersMiddlewareConfiguration BuildDefaultConfiguration() 
{ 
    return SecureHeadersMiddlewareBuilder 
        .CreateBuilder() 
        .UseHsts() 
        .UseXFrameOptions() 
        .UseContentTypeOptions() 
        .UseContentDefaultSecurityPolicy() 
        .UsePermittedCrossDomainPolicies() 
        .UseReferrerPolicy() 
        .UseCacheControl() 
        .RemovePoweredByHeader() 
        .UseXssProtection() 
        .UseCrossOriginResourcePolicy() 
        .Build(); 
} 
```

{: .warning }
The default configuration is INCREDIBLY restrictive.

### Custom Configuration

In order to use a custom configuration, follow the same pattern (perhaps creating your own extension method to encapsulate it). In the following example, we've created a static method called `CustomConfiguration` within a helpers class (called `CustomSecureHeaderExtensions`) which returns a completely custom configuration:

``` csharp
public static CustomSecureHeaderExtensions
{
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
}
```

{: .note }
This is an example configuration

Then consume it in the following manner, within your `Program.cs`'s Middleware pipeline:

```csharp
app.UseSecureHeadersMiddleware(
    CustomSecureHeaderExtensions.CustomConfiguration()
);
```
