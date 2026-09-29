#!/usr/bin/env bash
#
# End-to-end checks for the agent skill install (issue #236). Builds throwaway consumer projects
# against a freshly packed OwaspHeaders.Core, then checks where the skill was, and was not, written.
#
# Usage: .github/scripts/agent-skill-smoke.sh <directory containing the packed .nupkg>
#
# Runs on Linux, macOS and Windows (Git Bash). Every consumer lives in a temporary directory
# outside this repository, so this repository's Directory.Build.props is never imported.

set -euo pipefail

feed="$(cd "${1:?usage: agent-skill-smoke.sh <nupkg directory>}" && pwd)"
nupkg=""
for candidate in "$feed"/OwaspHeaders.Core.*.nupkg; do
    [[ "$candidate" == *.symbols.nupkg ]] || nupkg="$candidate"
done
[[ -f "$nupkg" ]] || { echo "No OwaspHeaders.Core package found in $feed"; exit 1; }
version="$(basename "$nupkg" .nupkg)"
version="${version#OwaspHeaders.Core.}"

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

# Paths handed to dotnet inside a file or an environment variable, rather than as an argument,
# need converting to Windows form under Git Bash. Arguments are converted automatically.
native() { if command -v cygpath >/dev/null; then cygpath -w "$1"; else echo "$1"; fi; }

# A private package cache, so that a package already cached under this version from an earlier
# pack is never used in place of the one being tested.
packages="$work/nuget-packages"
NUGET_PACKAGES="$(native "$packages")"
export NUGET_PACKAGES
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

failures=0
pass() { echo "  PASS: $1"; }
fail() { echo "  FAIL: $1"; failures=$((failures + 1)); }

expect_file() { if [[ -f "$1" ]]; then pass "$2"; else fail "$2 (missing $1)"; fi; }
# A negative check means nothing unless the build really ran, so it also checks the output exists.
expect_no_skill() {
    local dll
    for dll in "$1"/*/bin/Debug/net10.0/*.dll; do
        [[ -f "$dll" ]] || { fail "$2 (the build produced no output)"; return; }
    done
    local found
    found="$(find "$1" -path '*skills/owaspheaders-core*' -not -path "$packages/*" 2>/dev/null || true)"
    if [[ -z "$found" ]]; then pass "$2"; else fail "$2 (found: $found)"; fi
}

# Runs dotnet with every CI marker the targets look for removed, since the runner sets them.
dotnet_local() { env -u CI -u GITHUB_ACTIONS -u TF_BUILD -u GITLAB_CI dotnet "$@"; }

# Creates a consumer repository at $1 with web projects named by the remaining arguments, each
# referencing the package directly. Warnings are errors, as they are for many consumers.
new_consumer() {
    local root="$1"; shift
    mkdir -p "$root"
    # nuget.org is there for targeting packs the installed SDKs may not carry. The mapping
    # guarantees OwaspHeaders.Core itself only ever comes from the package under test.
    cat > "$root/nuget.config" <<EOF
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$(native "$feed")" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="OwaspHeaders.Core" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
EOF
    cat > "$root/Directory.Build.props" <<'EOF'
<Project>
  <PropertyGroup>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <MSBuildTreatWarningsAsErrors>true</MSBuildTreatWarningsAsErrors>
  </PropertyGroup>
</Project>
EOF
    local project
    for project in "$@"; do
        mkdir -p "$root/$project"
        cat > "$root/$project/$project.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="OwaspHeaders.Core" Version="$version" />
  </ItemGroup>
</Project>
EOF
        cat > "$root/$project/Program.cs" <<'EOF'
using OwaspHeaders.Core.Extensions;

var app = WebApplication.CreateBuilder(args).Build();
app.UseSecureHeadersMiddleware();
app.MapGet("/", () => "Hello");
app.Run();
EOF
    done
}

git_init() { git -C "$1" init -q && git -C "$1" -c user.name=smoke -c user.email=smoke@example.com commit -q --allow-empty -m init; }

echo "Testing OwaspHeaders.Core $version from $feed"

echo "Package contents"
entries="$(unzip -l "$nupkg")"
for entry in build/OwaspHeaders.Core.targets skills/owaspheaders-core/SKILL.md skills/owaspheaders-core/.gitignore; do
    if grep -q " $entry\$" <<<"$entries"; then pass "nupkg contains $entry"; else fail "nupkg contains $entry"; fi
done
if grep -q " buildTransitive/" <<<"$entries"; then fail "nupkg has no buildTransitive/"; else pass "nupkg has no buildTransitive/"; fi

echo "Installs into a git repository"
repo="$work/installs"
new_consumer "$repo" App
git_init "$repo"
dotnet_local build "$repo/App" --nologo -v q
expect_file "$repo/.agents/skills/owaspheaders-core/SKILL.md" "SKILL.md in .agents/skills"
expect_file "$repo/.claude/skills/owaspheaders-core/SKILL.md" "SKILL.md in .claude/skills"
expect_file "$repo/.agents/skills/owaspheaders-core/.gitignore" ".gitignore in .agents/skills"
expect_file "$repo/.claude/skills/owaspheaders-core/.gitignore" ".gitignore in .claude/skills"
if cmp -s "$repo/.claude/skills/owaspheaders-core/SKILL.md" <(unzip -p "$nupkg" skills/owaspheaders-core/SKILL.md); then
    pass "installed SKILL.md matches the packed one"
else
    fail "installed SKILL.md matches the packed one"
fi
# Everything but the consumer's own sources, which a real repository would have committed.
status="$(git -C "$repo" status --porcelain --untracked-files=all | grep -v -e ' App/' -e ' nuget.config$' -e ' Directory.Build.props$' || true)"
if [[ -z "$status" ]]; then pass "git status shows nothing from the skill"; else fail "git status shows nothing from the skill: $status"; fi

echo "A rebuild leaves the installed files alone"
touch "$work/marker"
sleep 1
dotnet_local build "$repo/App" --nologo -v q
touched="$(find "$repo/.agents" "$repo/.claude" -type f -newer "$work/marker")"
if [[ -z "$touched" ]]; then pass "no installed file rewritten"; else fail "no installed file rewritten: $touched"; fi

echo "Opt-outs"
for property in OwaspHeadersCoreAgentSkill EnableEmbeddedAgentSkills; do
    repo="$work/optout-$property"
    new_consumer "$repo" App
    git_init "$repo"
    dotnet_local build "$repo/App" --nologo -v q "-p:$property=false"
    expect_no_skill "$repo" "$property=false installs nothing"
done

echo "CI detection"
for marker in CI GITHUB_ACTIONS TF_BUILD GITLAB_CI; do
    repo="$work/ci-$marker"
    new_consumer "$repo" App
    git_init "$repo"
    env -u CI -u GITHUB_ACTIONS -u TF_BUILD -u GITLAB_CI "$marker=true" dotnet build "$repo/App" --nologo -v q
    expect_no_skill "$repo" "$marker set installs nothing"
done
repo="$work/ci-ContinuousIntegrationBuild"
new_consumer "$repo" App
git_init "$repo"
dotnet_local build "$repo/App" --nologo -v q -p:ContinuousIntegrationBuild=true
expect_no_skill "$repo" "ContinuousIntegrationBuild=true installs nothing"

echo "Outside a git repository"
repo="$work/no-git"
new_consumer "$repo" App
dotnet_local build "$repo/App" --nologo -v q
expect_no_skill "$work/no-git" "nothing written without a repository"

echo "Custom destinations"
repo="$work/custom"
new_consumer "$repo" App
git_init "$repo"
cat > "$repo/Directory.Build.targets" <<'EOF'
<Project>
  <ItemGroup>
    <OwaspHeadersCoreAgentSkillDestination Include=".github/skills" />
  </ItemGroup>
</Project>
EOF
dotnet_local build "$repo/App" --nologo -v q
expect_file "$repo/.github/skills/owaspheaders-core/SKILL.md" "SKILL.md in the declared destination"
if [[ -e "$repo/.agents" || -e "$repo/.claude" ]]; then fail "declaring a destination replaces the defaults"; else pass "declaring a destination replaces the defaults"; fi

echo "Git worktree"
repo="$work/worktree-main"
new_consumer "$repo" App
git_init "$repo"
git -C "$repo" add -A && git -C "$repo" -c user.name=smoke -c user.email=smoke@example.com commit -q -m app
git -C "$repo" worktree add -q "$work/worktree" 2>/dev/null
dotnet_local build "$work/worktree/App" --nologo -v q
expect_file "$work/worktree/.claude/skills/owaspheaders-core/SKILL.md" "installs at the worktree root"
# Not expect_no_skill: the main checkout is never built, so it has no output to check for.
if [[ -z "$(find "$repo" -path '*skills/owaspheaders-core*')" ]]; then
    pass "leaves the main checkout alone"
else
    fail "leaves the main checkout alone"
fi

echo "Parallel build of two direct references"
repo="$work/parallel"
new_consumer "$repo" Api Worker
git_init "$repo"
(cd "$repo" && dotnet new sln -n Parallel -o . >/dev/null && dotnet sln add Api Worker >/dev/null)
slns=("$repo"/Parallel.sln*)
sln="${slns[0]}"
for attempt in 1 2 3 4 5; do
    rm -rf "$repo/.agents" "$repo/.claude" "$repo"/*/bin "$repo"/*/obj
    if dotnet_local build "$sln" --nologo -v q -m -warnaserror; then
        pass "clean parallel build $attempt"
    else
        fail "clean parallel build $attempt"
    fi
done
expect_file "$repo/.claude/skills/owaspheaders-core/SKILL.md" "SKILL.md after parallel builds"

echo
if [[ $failures -gt 0 ]]; then
    echo "$failures check(s) failed"
    exit 1
fi
echo "All checks passed"
