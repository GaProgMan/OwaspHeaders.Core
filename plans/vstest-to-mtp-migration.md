# VSTest → Microsoft Testing Platform (MTP) migration plan

## Motivation

VSTest has been the default `dotnet test` runner since .NET Core's inception. It's reflection-driven, relies on out-of-process orchestration via `vstest.console`, and carries enough architectural baggage that Microsoft built a replacement: **Microsoft Testing Platform** (MTP). MTP is open source, hosted directly inside the test project's executable, zero-reflection, supports AOT, and is the platform Microsoft is steering all future investment toward. With the .NET 10 SDK, `dotnet test` gained a first-class MTP mode (selected via `global.json`), and the xunit.v3 project ships native MTP support.

This migration is **optional** for this codebase. The VSTest path (which the xunit v2→v3 migration deliberately preserved) works, all 132 tests pass, CI is healthy, and there are no behavioural gains for a library of this size. The honest motivations for taking it on now are: (1) staying current with the platform Microsoft is investing in; (2) closing the loop on the v3 migration, which explicitly called MTP out as a deferred follow-up; (3) curiosity. None of those are forcing functions.

**Recommendation: don't execute this plan yet.** Revisit in 6–12 months once MTP becomes the `dotnet test` default and the ecosystem (coverage tooling, IDE integrations, CI report actions) has fully settled. This plan exists so the analysis is preserved when that time comes.

## Current state inventory

- **Test framework**: xunit.v3 3.2.2 (with native MTP support compiled in).
- **VSTest packages still installed**: `Microsoft.NET.Test.Sdk 18.6.0`, `xunit.runner.visualstudio 3.1.5`.
- **Coverage**: `coverlet.collector 10.0.1`, invoked via `--collect:"XPlat Code Coverage"` in `.github/workflows/dotnet.yml`. Cobertura XML feeds the `irongut/CodeCoverageSummary` step.
- **Test result reporting**: `--logger trx`, output goes to `coverage/`.
- **Filter**: `--filter "Category!=Performance"` excludes the one performance-tagged test in `ClearSiteDataIntegrationTests`.
- **`<OutputType>Exe</OutputType>`**: already set on the test csproj (from the v3 migration). MTP requires this; the prerequisite is in place.
- **`global.json`**: does not exist.

## Migration scope

Five concrete changes. The csproj and `global.json` parts are mechanical. The CI workflow part is where most of the work — and most of the risk — lives.

## Phase 1: Add `global.json`

Create `global.json` at the repo root:

```json
{
    "test": {
        "runner": "Microsoft.Testing.Platform"
    }
}
```

This selects MTP mode for `dotnet test` under the .NET 10 SDK. Without it, `dotnet test` falls back to VSTest mode even with `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` set, and you get warnings about silently-ignored VSTest options.

Note: `global.json` is also commonly used to pin the SDK version (`"sdk": { "version": "..." }`). This repo doesn't pin the SDK and probably shouldn't start here — the CI workflows install SDKs explicitly. The `test` section is the only thing this file needs.

## Phase 2: Update the test csproj

File: `tests/OwaspHeaders.Core.Tests/OwaspHeaders.Core.Tests.csproj`.

```diff
   <PropertyGroup>
     ...
     <TargetFrameworks>net10.0;net11.0</TargetFrameworks>
     <OutputType>Exe</OutputType>
+    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
     ...
   </PropertyGroup>

   <ItemGroup>
-    <PackageReference Include="coverlet.collector" Version="10.0.1">
-      <PrivateAssets>all</PrivateAssets>
-      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
-    </PackageReference>
+    <PackageReference Include="Microsoft.Testing.Extensions.CodeCoverage" Version="18.1.0" />
+    <PackageReference Include="Microsoft.Testing.Extensions.TrxReport" Version="2.0.0" />

     <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="10.0.8" />
     <PackageReference Include="Microsoft.Extensions.Diagnostics.Testing" Version="10.6.0" />
     <PackageReference Include="xunit.v3" Version="3.2.2" />
     <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.6.0" />
     <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5">
       ...
     </PackageReference>
     ...
   </ItemGroup>
```

(Verify current stable versions of the two new extension packages at migration time. The xunit.net MTP docs explicitly recommend keeping `Microsoft.NET.Test.Sdk` and `xunit.runner.visualstudio` in place during the transition — they don't conflict with MTP and they keep VS/Rider's "click to run test" working for users on older IDE versions. Dropping them is a follow-up once the team's IDE versions have caught up.)

**Coverage decision: Microsoft.Testing.Extensions.CodeCoverage vs coverlet.MTP**

- `Microsoft.Testing.Extensions.CodeCoverage` is Microsoft's native MTP coverage extension. Closed-source-free-to-use licensing. Supports Cobertura output (`--coverage-output-format cobertura`).
- `coverlet.MTP` is coverlet's MTP integration. OSS. Supports Cobertura output (`--coverlet-output-format cobertura`).

Either works for our pipeline since both can emit Cobertura, which is what `irongut/CodeCoverageSummary` consumes. The plan recommends Microsoft's native because it's the "default" path MTP docs steer toward and avoids needing to learn coverlet's separate filter syntax. If you'd prefer the OSS licensing, swap to `coverlet.MTP` and adjust the CI flags accordingly.

**Important coverage gotcha**: `Microsoft.Testing.Extensions.CodeCoverage` defaults `IncludeTestAssembly` to `false` (VSTest's coverlet defaulted to `true`). Test projects are excluded from coverage by default — which is generally what you want, but it means the test assembly itself won't appear in reports. The current behaviour already excludes it via the `[assembly: ExcludeFromCodeCoverage]` attribute in the test csproj, so this is a no-op for us.

## Phase 3: Update CI workflows

The three workflows in `.github/workflows/` all invoke `dotnet test` and need their flags reworked. The most-touched is `dotnet.yml`'s `test` job.

**Current** (`dotnet.yml`):

```yaml
- name: Run all tests
  run: dotnet test OwaspHeaders.Core.sln --verbosity minimal --collect:"XPlat Code Coverage" --logger trx --results-directory coverage --filter "Category!=Performance"
```

**After**:

```yaml
- name: Run all tests
  run: dotnet test --solution OwaspHeaders.Core.sln --verbosity minimal --coverage --coverage-output-format cobertura --report-trx --results-directory coverage --filter-trait "Category!=Performance"
```

Five changes:

1. **`OwaspHeaders.Core.sln` → `--solution OwaspHeaders.Core.sln`**. In MTP mode of `dotnet test`, the solution/project path is passed as an explicit flag rather than a positional argument. Same for `--project` and `--test-modules` if you were passing those.
2. **`--collect:"XPlat Code Coverage"` → `--coverage --coverage-output-format cobertura`**. Drives `Microsoft.Testing.Extensions.CodeCoverage`. Default output goes to `TestResults/<guid>.coverage` unless `--coverage-output <path>` is specified.
3. **`--logger trx` → `--report-trx`**. Provided by `Microsoft.Testing.Extensions.TrxReport`. The default TRX filename is auto-generated; `--report-trx-filename <name>` overrides it.
4. **`--filter "Category!=Performance"` → `--filter-trait "Category!=Performance"`**. xunit.v3 uses `--filter-trait` for trait-based filtering; the bare `--filter` is reserved for fully-qualified-name filtering. The trait value's syntax is the same.
5. **`--results-directory coverage`** still works under MTP — the flag name is unchanged.

`codeql.yml` and `release.yml` also call `dotnet test`. The codeql one currently runs `dotnet test OwaspHeaders.Core.sln --verbosity minimal --filter "Category!=Performance"` — same treatment, drop the `--collect` and translate the filter. The release workflow's test job is a stripped-down version of the same; identical fix.

**Downstream CI step**: the `Code Coverage Summary Report` step uses `irongut/CodeCoverageSummary@v1.3.0` with `filename: 'coverage/*/coverage.cobertura.xml'`. Worth verifying the Microsoft coverage extension's Cobertura output file name and location match this glob. Likely it lands in `coverage/<guid>.cobertura` or similar — the wildcard should still catch it, but spot-check the first CI run.

## Phase 4: Validation

1. `dotnet restore --force-evaluate` — the lock file changes (coverlet swap + two new extension packages).
2. `dotnet build` — should succeed with no new warnings. `TreatWarningsAsErrors` will catch anything subtle.
3. `dotnet test` (local) — 132 tests pass.
4. Check that `coverage/*/coverage.cobertura.xml` (or similar) is produced and parses.
5. Push the branch and watch the CI run. The most likely failure modes are: (a) coverage file path mismatch with the `CodeCoverageSummary` action; (b) some flag was renamed or moved between MTP minor versions; (c) TRX output landing somewhere unexpected.
6. Verify Rider can still discover and run tests. (Modern Rider supports MTP natively; older Rider may not.)

## Known risks

- **Coverage output shape drift**: Microsoft's Cobertura serialiser may produce slightly different XML attribute ordering / namespace declarations than coverlet's. The `irongut/CodeCoverageSummary` action parses Cobertura without strict-schema validation, so it should be tolerant — but spot-checking the markdown summary it produces is worth a minute.
- **MTP version compatibility matrix**: `Microsoft.Testing.Extensions.CodeCoverage` has documented version pairings with the underlying MTP runtime (18.x ↔ MTP 2.0.x, 17.14.x ↔ MTP 1.6.2, etc.). xunit.v3 3.2.2 ships with a specific MTP version; adopting a coverage extension version misaligned with that ships the test project in an unsupported configuration. Pick versions tested together — the safe approach is "latest of each from the same week" and verify locally.
- **CodeQL workflow**: uses `build-mode: none` and manually runs `dotnet test` for analysis purposes. The CodeQL action doesn't care about test results per se, but the `dotnet test` invocation needs the same MTP flag treatment as the other workflows. Easy to forget.
- **IDE drift**: anyone on Rider <2024.3 or Visual Studio <17.13 will lose "click to run test" until they upgrade. Not a blocker but worth checking before merging.
- **`Microsoft.NET.Test.Sdk` and `xunit.runner.visualstudio` retention**: the plan recommends keeping both during the transition for IDE compatibility. A future "cleanup" PR can drop them once everyone's IDE supports MTP natively — but doing it in the same PR as the runner switch couples two risks and makes rollback harder.

## What's NOT changing

- The 132 tests themselves. No source file under `tests/.../*.cs` needs editing.
- `xunit.v3` package version — stays at 3.2.2.
- `Microsoft.Extensions.Diagnostics.Testing` (FakeLogger) — unchanged.
- `<OutputType>Exe</OutputType>` — already in place.
- The library code under `src/` — completely unaffected.

## Out of scope

- Dropping `Microsoft.NET.Test.Sdk` and `xunit.runner.visualstudio` after the migration settles. Worth a future minor follow-up once IDE/CI compatibility is confirmed.
- Adopting coverlet.MTP instead of Microsoft's native coverage extension. Documented above as an alternative; switching is a one-package swap with corresponding CI flag change.
- Pinning the SDK version in `global.json`. Currently the SDK is installed explicitly in CI; pinning here adds a constraint without benefit.
- Migrating to Native AOT. MTP supports it but the library doesn't have AOT-incompatible code paths that would benefit.

## Validation checklist

- [ ] `global.json` exists at repo root with the `test.runner` section.
- [ ] `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` added to test csproj.
- [ ] `coverlet.collector` removed; `Microsoft.Testing.Extensions.CodeCoverage` and `Microsoft.Testing.Extensions.TrxReport` added.
- [ ] `dotnet build` is green on both `net10.0` and `net11.0` with no warnings.
- [ ] `dotnet test` runs all 132 tests locally and reports them as passing.
- [ ] Cobertura coverage XML is produced and consumed by the `irongut/CodeCoverageSummary` step in `dotnet.yml`.
- [ ] TRX report is produced (visible in the workflow artifacts).
- [ ] `dotnet.yml`, `release.yml`, `codeql.yml` test steps all pass on the first CI run after the PR opens.
- [ ] Rider (or whichever IDE you primarily use) still discovers and runs tests without issue.
