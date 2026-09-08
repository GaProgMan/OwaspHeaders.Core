using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Tests.BuilderTests;

/// <summary>
/// Covers <see cref="SecureHeadersBuilder"/> directly, concentrating on the methods whose
/// behaviour is more than "set a flag and allocate a configuration object".
/// </summary>
public class SecureHeadersBuilderTests
{
    [Fact]
    public void NewBuilder_ProducesAnEmptyConfiguration()
    {
        // arrange, act
        var config = new SecureHeadersBuilder().Build();

        // assert
        Assert.False(config.UseHsts);
        Assert.False(config.UseXContentTypeOptions);
        Assert.Empty(config.UrlsToIgnore);
        Assert.NotNull(config.LoggingConfiguration);
    }

    [Fact]
    public void Build_ReturnsTheSameInstanceOnRepeatedCalls()
    {
        // arrange
        var builder = new SecureHeadersBuilder().UseHsts();

        // act
        var first = builder.Build();
        var second = builder.Build();

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void UseRecommendedDefaults_MatchesBuildDefaultConfiguration()
    {
        // arrange
        var viaPreset = new SecureHeadersBuilder().UseRecommendedDefaults().Build();
        // Comparing against the deprecated helper is the whole point: UseRecommendedDefaults
        // took over its body, and this asserts the two have not drifted while both ship.
#pragma warning disable CS0618
        var viaDefaults = SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration();
#pragma warning restore CS0618

        // act
        var presetHeaders = HeadersFor(viaPreset);
        var defaultHeaders = HeadersFor(viaDefaults);

        // assert
        Assert.Equal(defaultHeaders, presetHeaders);
        Assert.NotEmpty(presetHeaders);
    }

    [Fact]
    public void UseRecommendedDefaults_DoesNotEnableReportingEndpoints()
    {
        // arrange, act
        var config = new SecureHeadersBuilder().UseRecommendedDefaults().Build();

        // assert
        // The OWASP Secure Headers project does not recommend Reporting-Endpoints yet.
        Assert.False(config.UseReportingEndPoints);
    }

    [Fact]
    public void SetUrlsToIgnore_WithNull_LeavesTheExistingListAlone()
    {
        // arrange
        var builder = new SecureHeadersBuilder().SetUrlsToIgnore(["/skipthis"]);

        // act
        var config = builder.SetUrlsToIgnore(null).Build();

        // assert
        Assert.NotNull(config.UrlsToIgnore);
        Assert.Equal(["/skipthis"], config.UrlsToIgnore);
    }

    [Fact]
    public void UseClearSiteData_WithNoOptions_UsesTheOwaspRecommendedDefaults()
    {
        // arrange, act
        var config = new SecureHeadersBuilder().UseClearSiteData().Build();

        // assert
        Assert.True(config.UseClearSiteData);
        var headerValue = config.ClearSiteDataPathConfiguration.DefaultConfiguration.BuildHeaderValue();
        Assert.Contains("cache", headerValue);
        Assert.Contains("cookies", headerValue);
        Assert.Contains("storage", headerValue);
    }

    [Fact]
    public void AddClearSiteDataPath_WithEmptyOptions_Throws()
    {
        // arrange
        var builder = new SecureHeadersBuilder();

        // act
        var exception = Record.Exception(() => builder.AddClearSiteDataPath("/logout"));

        // assert
        // Unlike UseClearSiteData, this method does not substitute defaults for an empty array.
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);
    }

    [Fact]
    public void AddClearSiteDataPath_WithNullOrWhitespacePath_Throws()
    {
        // arrange
        var builder = new SecureHeadersBuilder();

        // act
        var exception = Record.Exception(() =>
            builder.AddClearSiteDataPath("  ", ClearSiteDataOptions.cookies));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentException>(exception);
    }

    [Fact]
    public void AddClearSiteDataPath_CalledRepeatedly_AccumulatesEveryPath()
    {
        // arrange, act
        var config = new SecureHeadersBuilder()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.cookies)
            .AddClearSiteDataPath("/signout", ClearSiteDataOptions.cache)
            .AddClearSiteDataPath("/account/close", ClearSiteDataOptions.storage)
            .Build();

        // assert
        Assert.True(config.UseClearSiteData);
        var paths = config.ClearSiteDataPathConfiguration.PathConfigurations;
        Assert.Equal(3, paths.Count);
        Assert.Contains("/logout", paths.Keys);
        Assert.Contains("/signout", paths.Keys);
        Assert.Contains("/account/close", paths.Keys);
    }

    [Fact]
    public void AddClearSiteDataPath_AfterUseClearSiteDataForPaths_PreservesTheDefaultConfiguration()
    {
        // arrange
        var builder = new SecureHeadersBuilder().UseClearSiteDataForPaths(
            new Dictionary<string, ClearSiteDataOptions[]>
            {
                { "/logout", [ClearSiteDataOptions.cookies] }
            },
            [ClearSiteDataOptions.cache]);

        // act
        var config = builder.AddClearSiteDataPath("/signout", ClearSiteDataOptions.storage).Build();

        // assert
        Assert.Equal(2, config.ClearSiteDataPathConfiguration.PathConfigurations.Count);
        Assert.NotNull(config.ClearSiteDataPathConfiguration.DefaultConfiguration);
        Assert.Contains("cache",
            config.ClearSiteDataPathConfiguration.DefaultConfiguration.BuildHeaderValue());
    }

    [Fact]
    public void AddClearSiteDataPath_CalledTwiceForTheSamePath_OverwritesRatherThanDuplicating()
    {
        // arrange, act
        var config = new SecureHeadersBuilder()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.cookies)
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.storage)
            .Build();

        // assert
        var paths = config.ClearSiteDataPathConfiguration.PathConfigurations;
        Assert.Single(paths);
        Assert.Contains("storage", paths["/logout"].BuildHeaderValue());
    }

    [Fact]
    public void UseClearSiteDataForPaths_WithNullDictionary_Throws()
    {
        // arrange
        var builder = new SecureHeadersBuilder();

        // act
        var exception = Record.Exception(() => builder.UseClearSiteDataForPaths(null));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentNullException>(exception);
    }

    [Fact]
    public void UseDefaultContentSecurityPolicy_PopulatesScriptSrcAndObjectSrc()
    {
        // arrange, act
        var config = new SecureHeadersBuilder().UseDefaultContentSecurityPolicy().Build();

        // assert
        Assert.True(config.UseContentSecurityPolicy);
        var headerValue = config.ContentSecurityPolicyConfiguration.BuildHeaderValue();
        Assert.Contains("script-src 'self'", headerValue);
        Assert.Contains("object-src 'self'", headerValue);
    }

    [Fact]
    public void SetCspUris_WithoutAContentSecurityPolicy_IsANoOp()
    {
        // arrange
        var builder = new SecureHeadersBuilder();

        // act
        var config = builder.SetCspUris(
            [ContentSecurityPolicyHelpers.CreateSelfDirective()], CspUriType.Script).Build();

        // assert
        Assert.False(config.UseContentSecurityPolicy);
        Assert.Null(config.ContentSecurityPolicyConfiguration);
    }

    [Fact]
    public void SetCspSandBox_AfterUseContentSecurityPolicy_IsApplied()
    {
        // arrange, act
        var config = new SecureHeadersBuilder()
            .UseContentSecurityPolicy()
            .SetCspSandBox(CspSandboxType.allowForms, CspSandboxType.allowScripts)
            .Build();

        // assert
        var headerValue = config.ContentSecurityPolicyConfiguration.BuildHeaderValue();
        Assert.Contains("sandbox", headerValue);
        Assert.Contains("allow-forms", headerValue);
    }

    [Fact]
    public void WithLoggingEventIds_WithNull_Throws()
    {
        // arrange
        var builder = new SecureHeadersBuilder();

        // act
        var exception = Record.Exception(() => builder.WithLoggingEventIds(null));

        // assert
        Assert.NotNull(exception);
        Assert.IsAssignableFrom<ArgumentNullException>(exception);
    }

    [Fact]
    public void WithLoggingEventIdBase_OffsetsEveryEventId()
    {
        // arrange, act
        var config = new SecureHeadersBuilder().WithLoggingEventIdBase(5000).Build();

        // assert
        Assert.Equal(5001, config.LoggingConfiguration.MiddlewareInitialized.Id);
    }

    private static List<string> HeadersFor(SecureHeadersMiddlewareConfiguration config)
    {
        var headers = new List<string>();
        foreach (var property in typeof(SecureHeadersMiddlewareConfiguration).GetProperties())
        {
            if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Use"))
            {
                headers.Add($"{property.Name}={property.GetValue(config)}");
            }
        }
        headers.Sort();
        return headers;
    }
}
