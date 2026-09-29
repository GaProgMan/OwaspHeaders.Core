---
title: Agent Skill
layout: page
nav_order: 6
---

# Agent Skill

{: .warning }
From version 11, **building a project which references OwaspHeaders.Core writes files into your repository.** It installs an [agent skill](https://agentskills.io/) for coding agents such as Claude Code, GitHub Copilot and Codex. This page covers what is written, why, when it is not, and how to turn it off.

## What is written, and where

When a project which references OwaspHeaders.Core builds, the package copies two files into each of two folders at the root of your git repository:

| Folder | Read by |
|---|---|
| `.agents/skills/owaspheaders-core/` | GitHub Copilot, Codex, and other agents following the cross-agent convention |
| `.claude/skills/owaspheaders-core/` | Claude Code (and GitHub Copilot) |

The two files are:

- `SKILL.md`, the skill itself: roughly a page of markdown.
- `.gitignore`, containing `*`.

Nothing else is written, and nothing is fetched over the network. Both files come from inside the NuGet package, so the guidance always matches the version you have installed.

## Why a security package does this

Coding agents learn from code written against older versions of libraries, and version 11 breaks exactly the code an agent trained on versions 8 to 10 will write: the configuration setters are now `internal`, `UseExpectCt` is gone, and `CreateBuilder()`/`Build()` is deprecated. Worse, the most damaging mistakes an agent can make with this package compile cleanly. Setting a security header by hand in a custom middleware, or adding `'unsafe-inline'` to a Content-Security-Policy to make a page work, builds, passes tests, and quietly weakens the protection the package exists to provide.

The skill puts version-specific guidance where agents look for it, so they use the middleware instead of working around it. It tells them:

- never to write the headers this middleware owns by hand, and why
- how to configure the middleware in version 11, and what to replace code from version 10 or earlier with
- where the middleware belongs in the request pipeline
- how to change the Content-Security-Policy without weakening it
- what the package does not do: Blazor and WebAssembly applications, and the `Server` header

### Answering "why is a security package writing files into my repository?"

This is a fair question for a security review to ask. The short answer:

- **What is written:** two small, human-readable text files, in two folders under your repository root. You can read them before trusting them; they are also in the package, under `skills/owaspheaders-core/`.
- **How:** a plain MSBuild `Copy` task in `build/OwaspHeaders.Core.targets`, which is also in the package. No compiled code runs during your build, no scripts are executed, and nothing is downloaded.
- **Where it never writes:** outside your git repository, on CI, or anywhere else listed under [When nothing is written](#when-nothing-is-written).
- **Source control:** the shipped `.gitignore` hides both folders from git, so nothing appears in `git status` and nothing is committed unless you choose to.
- **Control:** a single property turns it off. See [Opting out](#opting-out).

## Opting out

Set either property in your project file, or in `Directory.Build.props` to cover every project in the repository:

```xml
<PropertyGroup>
  <!-- Turns off this package's skill only -->
  <OwaspHeadersCoreAgentSkill>false</OwaspHeadersCoreAgentSkill>
</PropertyGroup>
```

```xml
<PropertyGroup>
  <!-- Turns off the skill of every package which honours this shared name -->
  <EnableEmbeddedAgentSkills>false</EnableEmbeddedAgentSkills>
</PropertyGroup>
```

`EnableEmbeddedAgentSkills` is a name proposed for all packages which ship skills this way. No convention has been agreed yet, so other packages may not honour it.

{: .important }
Opting out stops future builds writing the files, but **does not remove a skill which is already installed.** Delete `.agents/skills/owaspheaders-core/` and `.claude/skills/owaspheaders-core/` yourself.

## Choosing where it is installed

To install into different folders, declare `OwaspHeadersCoreAgentSkillDestination` items, each a skills folder relative to the repository root. Declaring any replaces both defaults. For example, to install for Claude Code only:

```xml
<ItemGroup>
  <OwaspHeadersCoreAgentSkillDestination Include=".claude/skills" />
</ItemGroup>
```

GitHub Copilot reads `.agents/skills`, `.claude/skills` and `.github/skills`, so with the defaults it may list the skill twice. Installing into a single folder avoids that.

## When nothing is written

The skill is **not** installed:

- **On CI.** A build counts as CI when any of these are set:
  - `ContinuousIntegrationBuild` is `true`, including when it is set in `Directory.Build.targets`
  - the `CI` environment variable is `true` or `1`
  - `GITHUB_ACTIONS` (GitHub Actions), `TF_BUILD` (Azure Pipelines) or `GITLAB_CI` (GitLab) is `true`

  This detection is a heuristic. A CI system which sets none of these, such as some Jenkins or TeamCity configurations, is treated as a developer machine, so opt out there.
- **In IDE design-time builds**, the background builds IDEs run to power IntelliSense. It is installed on the first real build.
- **Outside a git repository.** The repository root is the nearest folder above the project containing `.git`, which may be a folder or, in a worktree or submodule, a file. Without one, nothing is written anywhere.
- **When the repository root is your home folder**, for example when you keep your dotfiles in git. `~/.claude/skills` is where Claude Code looks for skills which apply to every project, so the package never writes there.
- **In projects which get the package only transitively.** Only a project with a direct `PackageReference` to OwaspHeaders.Core installs the skill.

### Projects with several target frameworks

A project with several frameworks in `<TargetFrameworks>` builds once per framework, often in parallel, and each build would otherwise copy the same files at the same moment. So only the build for the **first** framework listed installs the skill.

This means building only a later framework does not install it. With `<TargetFrameworks>net10.0;net11.0</TargetFrameworks>`, running `dotnet build -f net11.0` or `dotnet run -f net11.0` writes nothing. The next build of `net10.0`, or of every framework, installs it as normal.

## Things worth knowing

### Edits to the installed skill are overwritten

A build leaves the installed files alone while they match the package's copy, and replaces them when they do not. So **any local edit is overwritten on the next build**, and upgrading the package replaces the files with the new version's guidance. To use different guidance, opt out and keep your own skill under a different name.

### A copy which cannot write fails the build

If the files cannot be written, for example because the source tree is read-only on a CI system which is not detected, the build fails with `MSB3021`. That is deliberate. The alternatives were to report the failure as a warning, which would fail every project building with warnings as errors, or to ship compiled code to run during your build. The fix is to [opt out](#opting-out).

### The build is silent about the skill

A normal build prints nothing when the skill is installed, skipped, or opted out of. To see what happened, build with detailed output and look for the `_OwaspHeadersCoreInstallAgentSkill` target:

```bash
dotnet build -v:d | grep -A3 _OwaspHeadersCoreInstallAgentSkill
```

### The installed files are executable on macOS and Linux

The installed files have the mode `-rwxr--r--` rather than `-rw-r--r--`. This comes from NuGet, not from this package: the files are `-rw-r--r--` inside the package, NuGet gives every file it extracts into its package cache this mode, whatever the package, and the copy keeps it. The files are markdown and a `.gitignore`, and git ignores them, so this is harmless.

### Several versions in one repository

If projects in one repository reference different versions of OwaspHeaders.Core, each build writes its own version of the skill, and whichever builds last wins.

### Committing the skill

To commit the skill, so that everyone working in the repository gets it without building first, add it with `--force`:

```bash
git add --force .agents/skills/owaspheaders-core/SKILL.md .claude/skills/owaspheaders-core/SKILL.md
```

Deleting the `.gitignore` instead does not work, because the next build puts it back. Once committed, `SKILL.md` stays tracked even though the `.gitignore` matches it, so when a package upgrade changes the guidance, the change shows up in `git status` for you to review.
