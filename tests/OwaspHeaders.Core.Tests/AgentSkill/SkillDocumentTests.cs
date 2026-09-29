using System.IO;
using System.Text.RegularExpressions;

namespace OwaspHeaders.Core.Tests.AgentSkill;

/// <summary>
/// Keeps the agent skill shipped in the package (src/AgentSkill/SKILL.md) true to the API.
/// </summary>
/// <remarks>
/// A skill containing wrong guidance is worse than no skill, because agents weight it above what
/// they already believe. So every C# sample in it must match a compiled sample in
/// <see cref="SkillSamples"/>, and the frontmatter must satisfy the Agent Skills spec, which
/// agents enforce by silently not loading the skill. See issue #236.
/// </remarks>
public partial class SkillDocumentTests
{
    private const string SkillName = "owaspheaders-core";

    // The frontmatter fields the Agent Skills spec defines. Claude Code accepts more, but some
    // agents reject a skill carrying any other field.
    private static readonly string[] SpecFrontmatterFields =
        ["name", "description", "license", "compatibility", "metadata", "allowed-tools"];

    private static readonly string Skill = ReadOutputFile("SKILL.md");
    private static readonly Dictionary<string, string> Fences = ReadFences(Skill);
    private static readonly Dictionary<string, string> Samples =
        ReadSamples(ReadOutputFile("SkillSamples.cs"));

    [Fact]
    public void EveryCSharpFence_IsPrecededByASampleMarker()
    {
        var fenceCount = CSharpFenceRegex().Matches(Skill).Count;

        Assert.NotEqual(0, fenceCount);
        Assert.Equal(fenceCount, Fences.Count);
    }

    [Fact]
    public void EveryFence_MatchesItsCompiledSample()
    {
        Assert.All(Fences, fence =>
        {
            Assert.True(Samples.TryGetValue(fence.Key, out var sample),
                $"SKILL.md has a '{fence.Key}' sample with no matching region in SkillSamples.cs");
            Assert.Equal(sample, fence.Value);
        });
    }

    [Fact]
    public void EveryCompiledSample_AppearsInTheSkill()
    {
        Assert.Equal(Samples.Keys.Order(), Fences.Keys.Order());
    }

    [Fact]
    public void Frontmatter_SatisfiesTheAgentSkillsSpec()
    {
        var frontmatter = ReadFrontmatter(Skill);

        Assert.All(frontmatter.Keys, key => Assert.Contains(key, SpecFrontmatterFields));

        // The name must also match the folder the skill is installed into, or agents will not
        // load it.
        var name = frontmatter["name"];
        Assert.Equal(SkillName, name);
        Assert.Contains($"/{SkillName}/", ReadOutputFile("OwaspHeaders.Core.targets"));
        Assert.Matches("^[a-z0-9]+(-[a-z0-9]+)*$", name);
        Assert.InRange(name.Length, 1, 64);

        Assert.InRange(frontmatter["description"].Length, 1, 1024);
    }

    [Fact]
    public void Skill_IsWithinTheRecommendedLength()
    {
        Assert.InRange(Skill.Split('\n').Length, 1, 500);
    }

    [Fact]
    public void Skill_NamesTheMajorVersionOfThePackage()
    {
        var major = typeof(SecureHeadersMiddleware).Assembly.GetName().Version!.Major;

        Assert.Contains($"OwaspHeaders.Core {major}.x", Skill);
    }

    [Fact]
    public void ShippedGitignore_IgnoresEverythingIncludingItself()
    {
        var rules = ReadOutputFile("skill.gitignore")
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .ToArray();

        // "*" alone also matches the .gitignore, so nothing in the directory is ever untracked.
        // A "!.gitignore" rule would leave the file untracked, and fail consumers who gate their
        // builds on a clean `git status --porcelain`.
        Assert.Equal(["*"], rules);
    }

    private static string ReadOutputFile(string fileName) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "AgentSkill", fileName)).ReplaceLineEndings("\n");

    private static Dictionary<string, string> ReadFrontmatter(string skill)
    {
        var match = FrontmatterRegex().Match(skill);
        Assert.True(match.Success, "SKILL.md must start with YAML frontmatter");

        return match.Groups["body"].Value
            .Split('\n')
            .Select(line => FrontmatterFieldRegex().Match(line))
            .Where(field => field.Success)
            .ToDictionary(field => field.Groups["key"].Value, field => field.Groups["value"].Value.Trim());
    }

    private static Dictionary<string, string> ReadFences(string skill) =>
        MarkedFenceRegex().Matches(skill)
            .ToDictionary(match => match.Groups["id"].Value, match => Normalise(match.Groups["code"].Value));

    private static Dictionary<string, string> ReadSamples(string source) =>
        SampleRegionRegex().Matches(source)
            .ToDictionary(match => match.Groups["id"].Value, match => Normalise(match.Groups["code"].Value));

    // Removes the indentation common to every line, trailing whitespace and surrounding blank
    // lines, so that a sample indented inside a method compares equal to the same code in a fence.
    private static string Normalise(string code)
    {
        var lines = code.Split('\n').Select(line => line.TrimEnd()).ToList();
        while (lines.Count > 0 && lines[0].Length == 0)
        {
            lines.RemoveAt(0);
        }

        while (lines.Count > 0 && lines[^1].Length == 0)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        var indent = lines.Where(line => line.Length > 0)
            .Select(line => line.Length - line.TrimStart().Length)
            .DefaultIfEmpty(0)
            .Min();

        return string.Join('\n', lines.Select(line => line.Length == 0 ? line : line[indent..]));
    }

    [GeneratedRegex(@"\A---\n(?<body>.*?)\n---\n", RegexOptions.Singleline)]
    private static partial Regex FrontmatterRegex();

    [GeneratedRegex(@"^(?<key>[a-z][a-z-]*):(?<value>.*)$")]
    private static partial Regex FrontmatterFieldRegex();

    [GeneratedRegex(@"^```csharp\n", RegexOptions.Multiline)]
    private static partial Regex CSharpFenceRegex();

    [GeneratedRegex(@"^<!-- sample: (?<id>[a-z-]+) -->\n```csharp\n(?<code>.*?)^```$", RegexOptions.Multiline | RegexOptions.Singleline)]
    private static partial Regex MarkedFenceRegex();

    [GeneratedRegex(@"// <skill-sample:(?<id>[a-z-]+)>\n(?<code>.*?)\n\s*// </skill-sample:\k<id>>", RegexOptions.Singleline)]
    private static partial Regex SampleRegionRegex();
}
