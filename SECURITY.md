# Security Policy

We minimise the security risk when developing code for, and supplying it to consumers of, OwaspHeaders.Core by making all source code available, using only official Microsoft-backed NuGet dependencies, carefully reviewing all code contributions, building and publishing packages in the cloud with GitHub Actions, and using deterministic builds and SourceLink for all packages deployed to NuGet.

This security policy is based on the one found in the repository for [ScottPlot](https://github.com/ScottPlot/ScottPlot/blob/main/SECURITY.md)

## Supported Versions

We support any release of OwaspHeaders.Core that is running on a version of .NET which Microsoft still supports. A release which targets a .NET version that has reached end of support is itself unsupported, regardless of its version number.

This means that support for a given release ends on the day that the newest version of .NET it targets leaves Microsoft's [support lifecycle](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), whether or not this file has been updated to say so.

The table below applies that rule to the current releases:

| Version | Targets | Supported |
| ------- | ------- | --------- |
| 10.x.x | .NET 8, 9, 10 | :white_check_mark: until 14 November 2028 |
| 9.x.x | .NET 8, 9 | :white_check_mark: until 10 November 2026 |
| 8.x.x | .NET 6, 7, 8 | :white_check_mark: until 10 November 2026, **on .NET 8 only** |
| < 8.0.0 | .NET 6 and earlier | :x: |
| [framework](https://github.com/GaProgMan/OwaspHeaders.Core/releases/tag/framework) | .NET Framework, ASP .NET Core 2.2 | :x: |

Two consequences of the rule are worth calling out:

- **8.x.x is only supported when run on .NET 8.** It also targets .NET 6 and .NET 7, both of which Microsoft has retired. If you are consuming 8.x.x on either of those runtimes, you are unsupported today, and the fix is to move to a supported .NET version rather than to a newer package version.
- **.NET 8 and .NET 9 both reach end of support on 10 November 2026.** On that date, 8.x.x and 9.x.x become unsupported together, and 10.x.x continues by way of its .NET 10 target.

Backporting features and fixes to unsupported releases is not provided.

## Reporting a Vulnerability

Please report vulnerabilities using GitHub's [Private Vulnerability Reporting](https://docs.github.com/en/code-security/how-tos/report-and-fix-vulnerabilities/report-privately) functionality.

We will aim to acknowledge any vulnerabilities within 72 hours, and fix them as soon as practicable, with severity-dependent timelines.
