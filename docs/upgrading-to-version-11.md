---
title: Upgrading to version 11
layout: page
nav_order: 2
---

# Upgrading to version 11

Version 11 is a major release. This page covers what you need to do to move a project from version 10: the code to change, what to expect on the first build, and how to opt out of the parts you do not want. The [changelog](./changelog#version-11) records what changed and why, and each section below links to its entry there.

Changes fall into two groups:

- [**Breaking API changes**](#breaking-api-changes) stop your code compiling until you change it.
- [**Behavioural changes**](#behavioural-changes) still compile, but behave differently at runtime, produce new warnings, or write files you should know about.

If you are upgrading from a version earlier than 10.5.0, also read [Coming from a version earlier than 10.5.0](#coming-from-a-version-earlier-than-1050).

## Before you start

Version 11 targets .NET 10 and .NET 11 only. If your project cannot move off .NET 8 or .NET 9 yet, stay on version 10 for now. See the [security policy](https://github.com/GaProgMan/OwaspHeaders.Core/blob/main/SECURITY.md#supported-versions) for how long each release is supported.

A suggested order of work:

1. Update the package, then build. Fix the errors listed under [Breaking API changes](#breaking-api-changes).
2. Work through the new warnings: [`CS0618`](#the-build-then-pass-api-is-deprecated-cs0618) for the deprecated configuration API, and, if you build with nullable enabled, [`CS8602`](#new-nullable-warnings) and friends.
3. Start the application and run your tests. [Configuration is now validated at startup](#an-invalid-configuration-stops-the-application-from-starting), so a problem which used to appear on the first request now stops the host from starting.
4. Check your repository for the [agent skill](#an-agent-skill-is-installed-on-build) the package now installs, and opt out if you do not want it.

## Breaking API changes

Each of these fails to compile, or to restore, against version 11.

### Your project targets .NET 8 or .NET 9

Restoring fails with `NU1202`, because the package no longer contains a build for your target framework. Change `<TargetFramework>` to `net10.0` or `net11.0`, or stay on version 10.

Changelog: [Supported runtimes](./changelog#supported-runtimes).

### `UseExpectCt` no longer exists

The Expect-CT header has been removed entirely, following its deprecation by OWASP and the `[Obsolete]` marker added in 10.3. Delete any call to `.UseExpectCt(...)` and any use of `UseExpectCt` or `ExpectCt` on a configuration object. Nothing replaces it, and you do not need anything to.

Changelog: [Expect-CT removal](./changelog#expect-ct-removal).

### Configuration properties can no longer be assigned (`CS0272`)

Every property on `SecureHeadersMiddlewareConfiguration` now has an `internal` setter. Code which switched a header on by assigning a flag or a configuration object no longer compiles:

```csharp
// version 10: compiles, but throws on the first request from 10.4 onwards
var config = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
config.UseCacheControl = true;
```

Use the matching builder method instead:

```csharp
// version 11
app.UseSecureHeadersMiddleware(opt => opt.UseCacheControl());
```

The compiler error names the property. Each header has one builder method which sets both its flag and its configuration object, although the names do not always match exactly: `UseXContentTypeOptions` is set by `UseContentTypeOptions()`, for example. Reading the properties is unaffected.

Changelog: [Configuration setter lockdown](./changelog#configuration-setter-lockdown-issue-220).

### Guard clauses and extension helpers are internal (`CS0122`)

These types were public by accident and are now `internal`:

- `ObjectGuardClauses`, `HeaderValueGuardClauses` and `BoolValueGuardClauses`, in `OwaspHeaders.Core.Guards`
- `ArgumentExceptionHelper`, in `OwaspHeaders.Core.Helpers`
- `StringBuilderExtensions` and `HttpContextExtensions`, in `OwaspHeaders.Core.Extensions`

If you called any of them, replace the call with your own code or with a BCL equivalent. `ArgumentNullException.ThrowIfNull` and `ArgumentException.ThrowIfNullOrWhiteSpace` are the closest, but they are **not** drop-in replacements: `ThrowIfNullOrWhiteSpace` throws `ArgumentNullException` for null, where the library's guard threw `ArgumentException`, and `ThrowIfNull` does not take a custom message.

`OwaspHeaders.Core.Guards` now has no public types, so remove any `using OwaspHeaders.Core.Guards;` directive, even in a file which calls nothing from it. `ContentSecurityPolicyHelpers` is still public, so `using OwaspHeaders.Core.Helpers;` is fine.

Changelog: [Guard clauses and extension helpers are now internal](./changelog#guard-clauses-and-extension-helpers-are-now-internal-issue-233).

### Subclasses of the configuration models cannot call `base()` (`CS7036`)

The protected parameterless constructors have been removed from `HstsConfiguration`, `XFrameOptionsConfiguration`, `ContentSecurityPolicyConfiguration`, `ContentSecurityPolicySandBox`, `ReferrerPolicy`, `CacheControl`, `PermittedCrossDomainPolicyConfiguration`, `ReportingEndpointsPolicy` and `ClearSiteDataConfiguration`. A subclass of one of them must call a public constructor with parameters instead.

Changelog: [Model constructors, required members and validation](./changelog#model-constructors-required-members-and-validation-issue-233).

### `ContentSecurityPolicyElement` needs a `DirectiveOrUri` (`CS9035`)

`DirectiveOrUri` is now `required`, so an object initialiser which leaves it out no longer compiles. Set it:

```csharp
new ContentSecurityPolicyElement
{
    CommandType = CspCommandType.Uri,
    DirectiveOrUri = "https://cdn.example.com"
};
```

`CommandType` is still optional and still defaults to `CspCommandType.Directive`. Two side effects of `required` to be aware of: System.Text.Json enforces it when deserialising, and the type can no longer satisfy a generic `new()` constraint.

Changelog: [Model constructors, required members and validation](./changelog#model-constructors-required-members-and-validation-issue-233).

### `UseSecureHeadersMiddleware(null)` is ambiguous (`CS0121`)

A literal `null` now matches both the deprecated configuration overload and the new delegate overload. Remove the argument, since `app.UseSecureHeadersMiddleware()` applies the recommended defaults, or pass a delegate. A typed variable which might hold null is unaffected.

Changelog: [Configure the middleware with a delegate](./changelog#configure-the-middleware-with-a-delegate-issue-59).

## Behavioural changes

These still compile, although some produce warnings, which are errors if you build with `TreatWarningsAsErrors`.

### The build-then-pass API is deprecated (`CS0618`)

Configuring the middleware now means passing a delegate to `UseSecureHeadersMiddleware`, rather than building a configuration object and handing it over:

```csharp
// version 10, and still supported in version 11 with a CS0618 warning
var config = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .SetUrlsToIgnore(["/health"])
    .Build();

app.UseSecureHeadersMiddleware(config);
```

```csharp
// version 11
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseHsts();
    opt.UseXFrameOptions();
    opt.SetUrlsToIgnore(["/health"]);
});
```

To start from the OWASP recommended set and change individual headers, call `opt.UseRecommendedDefaults()` first; it replaces `BuildDefaultConfiguration()`. `app.UseSecureHeadersMiddleware()` with no arguments still applies the recommended set, and does not warn. If you need a configuration object itself, for example to inspect it, use `new SecureHeadersBuilder().UseHsts().Build()`, which is not deprecated.

Four members are marked `[Obsolete]`: `SecureHeadersMiddlewareBuilder.CreateBuilder`, `SecureHeadersMiddlewareBuilder.Build`, `BuildDefaultConfiguration`, and the `UseSecureHeadersMiddleware(config, urlIgnoreList)` overload. Three things to know about the warning they produce:

1. **The warning is deliberate.** It is how the package tells you to migrate.
2. **These members are removed in version 12**, along with the `UseX` extension methods that chain off `CreateBuilder()`. Those methods are not marked individually, so that a long chain produces two warnings rather than one per call, but they go at the same time.
3. **If you want it quiet while you migrate, silence it in your own project.** The package does not do this for you, because the only way it could reach your build would switch off obsoletion warnings for your whole project, including those from other packages and from your own code. Adding `CS0618` to `NoWarn` has that same effect, so keep it temporary:

   ```xml
   <PropertyGroup>
     <NoWarn>$(NoWarn);CS0618</NoWarn>
   </PropertyGroup>
   ```

   If you build with `TreatWarningsAsErrors` and would rather keep seeing the warning, `<WarningsNotAsErrors>$(WarningsNotAsErrors);CS0618</WarningsNotAsErrors>` stops it failing the build without hiding it.

One behaviour change to check while migrating: the deprecated overload ignored its `urlIgnoreList` argument whenever a configuration was also passed. `opt.SetUrlsToIgnore(...)` always applies, so if you passed both, the list now takes effect.

Changelog: [Configure the middleware with a delegate](./changelog#configure-the-middleware-with-a-delegate-issue-59).

### An invalid configuration stops the application from starting

Configuration is now validated while the request pipeline is built, so a configuration problem throws from `app.Build()` or `app.Run()` instead of on the first request. The exception is still an `ArgumentException`, but its wording has changed.

- If you caught this exception around request handling, it no longer arrives there.
- The `MiddlewareInitialized` (1001) and `HeadersGenerated` (1004) log entries are now written at startup, not on the first request. Update any integration test which asserts on log ordering.
- A configuration which enables no headers at all is still valid, but now logs a warning (event ID 2003) at startup.

You can now check a configuration from a unit test without starting a host. `SecureHeadersBuilder.BuildAndValidate` takes the same delegate as `UseSecureHeadersMiddleware`, so move the delegate into a named method and use it in both places:

```csharp
static void ConfigureSecureHeaders(SecureHeadersBuilder opt) => opt.UseRecommendedDefaults();

// in Program.cs
app.UseSecureHeadersMiddleware(ConfigureSecureHeaders);

// in a test: throws if the configuration is invalid
SecureHeadersBuilder.BuildAndValidate(ConfigureSecureHeaders);
```

Changelog: [Configuration is validated at startup](./changelog#configuration-is-validated-at-startup-issue-59).

### Report-only Content-Security-Policy is fixed

Three defects in report-only mode, present since it was added, are fixed. Two of the fixes change behaviour:

- **Report-only headers now include their directives.** `SetCspUris` and `SetCspSandBox` used to be ignored in report-only mode, so `Content-Security-Policy-Report-Only` carried no directives and reported nothing. It now carries what you configured, so **browsers will start sending violation reports to your `report-uri`**. Make sure that endpoint is ready for them. A report-only policy never blocks anything.
- **Passing `useXContentSecurityPolicy: true` to `UseContentSecurityPolicyReportUriOnly` or `UseContentSecurityPolicyReportOnly` now throws** when the pipeline is built. X-Content-Security-Policy has no report-only form. To emit it, pass `useXContentSecurityPolicy: true` to `UseContentSecurityPolicy` instead; a later report-only call no longer switches it off.

`SetCspUris` and `SetCspSandBox` only affect policies which already exist, so call them after the method which sets up the policy.

Changelog: [Report-only Content-Security-Policy fixes](./changelog#report-only-content-security-policy-fixes-issue-240).

### Invalid values are rejected when they are configured

Two kinds of input which used to produce a malformed header, or a crash later on, now throw as soon as they are created. Inside the configure delegate, that means at startup:

- A `ContentSecurityPolicyElement` whose `DirectiveOrUri` is empty or whitespace throws an `ArgumentException`.
- `UseReportingEndpointsPolicy` rejects a null dictionary, an endpoint name which is empty or whitespace, and a null `Uri`.

Changelog: [Model constructors, required members and validation](./changelog#model-constructors-required-members-and-validation-issue-233).

### New nullable warnings

This only affects projects which build with nullable reference types enabled. The whole public API is now annotated, so you get accurate warnings where you previously got none.

- **Reading a per-header configuration object without checking its flag produces `CS8602`.** Those objects really are null until their header is configured. Check the flag first, and the compiler knows the object is not null:

  ```csharp
  if (config.UseHsts)
  {
      var value = config.HstsConfiguration.BuildHeaderValue();
  }
  ```

- **Passing null where the library never accepted it now warns.** The runtime guard was already throwing for it, so the fix is to stop passing null.

Nothing changes at runtime.

Changelog: [The configuration object is nullable-annotated](./changelog#the-configuration-object-is-nullable-annotated-issue-233) and [Nullable reference types are enabled library-wide](./changelog#nullable-reference-types-are-enabled-library-wide-issue-233).

### An agent skill is installed on build

{: .warning }
**Building a project which references OwaspHeaders.Core now writes files into your repository.** Nothing stops compiling, but this is the change most likely to surprise you, so every rule is listed here. The [Agent Skill](./agent-skills) page explains why a security package does this, and goes into more detail.

The package installs an [agent skill](https://agentskills.io/) which gives coding agents such as Claude Code, GitHub Copilot and Codex the version 11 API, and tells them not to hand-write the headers the middleware owns.

**What is written, and where.** `SKILL.md` and a `.gitignore`, in `.agents/skills/owaspheaders-core/` and `.claude/skills/owaspheaders-core/` at the root of your git repository. A build leaves them alone while they match the package's copy, and replaces them when the package version changes. **Local edits are overwritten on the next build.** For different guidance, opt out and keep your own skill under a different name.

**Nothing shows up in git.** The `.gitignore` contains `*`, which ignores the folder's contents, itself included, so `git status --porcelain` stays clean. To commit the skill, add it with `git add --force`; it then stays tracked. Deleting the `.gitignore` does not work, because the next build puts it back.

**Opting out.** Either property goes in your project file, or in `Directory.Build.props` to cover every project:

```xml
<PropertyGroup>
  <!-- Turns off this package's skill only -->
  <OwaspHeadersCoreAgentSkill>false</OwaspHeadersCoreAgentSkill>
</PropertyGroup>
```

`<EnableEmbeddedAgentSkills>false</EnableEmbeddedAgentSkills>` turns off every package which honours that shared name. **Opting out stops future writes, but does not remove a skill which is already installed.** Delete `.agents/skills/owaspheaders-core/` and `.claude/skills/owaspheaders-core/` yourself.

**Choosing locations.** Declaring `OwaspHeadersCoreAgentSkillDestination` items, each a skills folder relative to the repository root, replaces both defaults. Agents which read both default locations, such as GitHub Copilot, which reads `.agents/skills`, `.claude/skills` and `.github/skills`, may list the skill twice; choosing a single location avoids that.

**When nothing is written:**

- On CI, detected by `ContinuousIntegrationBuild=true` (including when it is set in `Directory.Build.targets`), `CI=true` or `CI=1`, `GITHUB_ACTIONS`, `TF_BUILD` or `GITLAB_CI`. This detection is a heuristic, so on any other CI system, opt out.
- In IDE design-time builds.
- When the project is not inside a git repository.
- When the repository root is your home folder, for example a dotfiles repository.
- In projects which get the package only transitively. Only a direct `PackageReference` installs it.

**Projects with several target frameworks.** Only the build for the *first* framework in `<TargetFrameworks>` installs the skill, so parallel builds never write the same files at once. Building only a later framework, for example `dotnet build -f net11.0` or `dotnet run -f net11.0`, does not install it. The next build of the first framework, or of all of them, does.

**If the copy cannot write**, for example in a read-only source tree on a CI system which is not detected, the build fails with `MSB3021`. That is deliberate: the alternatives were a warning, which fails builds using warnings as errors, or shipping code which runs during your build. The fix is to opt out.

**Several versions in one repository.** If projects reference different versions of the package, whichever builds last writes its version of the skill.

**The installed files are executable on macOS and Linux** (`-rwxr--r--` rather than `-rw-r--r--`). This comes from NuGet, not from this package: NuGet gives every file it extracts into its package cache that mode, whatever the package, and the copy keeps it. The files are markdown and a `.gitignore`, and git ignores them, so this is harmless.

**The build is silent about the skill.** A normal build prints nothing when the skill is installed, skipped, or opted out of. `dotnet build -v:d` shows whether the `_OwaspHeadersCoreInstallAgentSkill` target ran.

Changelog: [An agent skill is installed on build](./changelog#an-agent-skill-is-installed-on-build-issue-236).

## Coming from a version earlier than 10.5.0

Version 10.5.0 changed how Cache-Control is built, so if you jump straight from an earlier version to 11 you get this change as well:

- The default value is now `no-cache, no-store, max-age=0`, which ASP.NET Core antiforgery accepts without overriding it or logging a warning. Browsers and proxies cache nothing either way.
- The options passed to `UseCacheControl` now combine, instead of the first one set winning. For example, `UseCacheControl(@private: true)` now produces `private, no-cache, no-store, max-age=0` rather than `private`.
- If you relied on the old behaviour to allow caching, pass `noCache: false` and `noStore: false` explicitly. `UseCacheControl(maxAge: N, noStore: false)` now produces `no-cache, max-age=N`, which effectively disables the `max-age`; add `noCache: false` to keep the old behaviour.

Changelog: [Version 10.5.x](./changelog#version-105x).
