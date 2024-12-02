# Security Policy

We minimise the security risk when developing code for, and supplying it to consumers of, OwaspHeaders.Core by making all source code available, using only official Microsoft-backed NuGet dependencies, carefully reviewing all code contributions, building and publishing packages in the cloud with GitHub Actions, and using deterministic builds and SourceLink for all packages deployed to NuGet.

This security policy is based on the one found on the one found in the repository for [ScottPlot](https://github.com/ScottPlot/ScottPlot/blob/main/SECURITY.md)

## Supported Versions

The following table contains the list of versions of OwaspHeaders.Core that are currently supported:

| Version | Supported          |
| ------- | ------------------ |
| 9.x.x   | :white_check_mark: |
| 8.x.x   | :white_check_mark: |
| 6.x.x   | 💵 &ast;           |
| < 6.0.0 | :x:                |
| [framework](https://github.com/GaProgMan/OwaspHeaders.Core/releases/tag/framework) | 💵 &ast; |

&ast; = paid support is available for this version, as Microsoft has dropped the version of .NET required or no longer supports that hosting situation (in the case of framework).

## Reporting a Vulnerability

Please report vulnerabilities using GitHub's [Issues](https://github.com/GaProgMan/OwaspHeaders.Core/issues) functionality.

We will aim to fix any vulnerabilities within 48-72 hours of them being reported.
