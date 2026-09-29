namespace OwaspHeaders.Core.Tests.AgentSkill;

/// <summary>
/// Runs the samples from the agent skill, to check that they do what the skill says they do.
/// Compiling them (see <see cref="SkillDocumentTests"/>) only proves the API exists.
/// </summary>
public class SkillSampleBehaviourTests
{
    [Fact]
    public async Task HandRolledHeader_SilentlyReplacesTheConfiguredValue()
    {
        // The skill tells agents not to set these headers by hand because a value already on the
        // response wins over the configured one. This is that claim.
        using var host = CreateHost(app =>
        {
            SkillSamples.HandRolledHeaders(app);
            app.UseSecureHeadersMiddleware(opt => opt.UseXFrameOptions(XFrameOptions.Sameorigin));
        });

        var response = await GetAsync(host.GetTestServer(), "/");

        Assert.Equal("DENY", response.Headers[Constants.XFrameOptionsHeaderName]);
    }

    [Fact]
    public async Task RecommendedDefaults_AddsTheRecommendedHeaders()
    {
        using var host = CreateHost(SkillSamples.RecommendedDefaults);

        var response = await GetAsync(host.GetTestServer(), "/");

        Assert.Equal("deny", response.Headers[Constants.XFrameOptionsHeaderName]);
        Assert.Equal("max-age=31536000;includeSubDomains", response.Headers[Constants.StrictTransportSecurityHeaderName]);
    }

    [Fact]
    public async Task ConfigureDelegate_AppliesTheChangeAndIgnoresTheListedPath()
    {
        using var host = CreateHost(SkillSamples.ConfigureDelegate);

        var response = await GetAsync(host.GetTestServer(), "/");
        var health = await GetAsync(host.GetTestServer(), "/health");

        Assert.Equal("max-age=63072000;includeSubDomains", response.Headers[Constants.StrictTransportSecurityHeaderName]);
        Assert.Equal("deny", response.Headers[Constants.XFrameOptionsHeaderName]);
        Assert.False(health.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
    }

    [Fact]
    public async Task SharedConfiguration_ValidatesAndApplies()
    {
        using var host = CreateHost(SkillSamples.SharedConfiguration);

        var response = await GetAsync(host.GetTestServer(), "/");
        var health = await GetAsync(host.GetTestServer(), "/health");

        Assert.Equal("deny", response.Headers[Constants.XFrameOptionsHeaderName]);
        Assert.False(health.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
    }

    [Fact]
    public async Task CspAllowOrigin_KeepsSelfAndAddsTheOrigin()
    {
        using var host = CreateHost(SkillSamples.CspAllowOrigin);

        var response = await GetAsync(host.GetTestServer(), "/");

        Assert.StartsWith("script-src 'self' https://cdn.example.com;",
            response.Headers[Constants.ContentSecurityPolicyHeaderName].ToString());
    }

    [Fact]
    public async Task PipelinePlacement_AddsHeadersToResponsesNoEndpointHandled()
    {
        // The reason for registering the middleware early: a response written by anything
        // registered after it, a 404 here, still carries the headers.
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers();

        await using var app = SkillSamples.PipelinePlacement(builder);
        await app.StartAsync(TestContext.Current.CancellationToken);

        var server = app.GetTestServer();
        server.BaseAddress = new Uri("https://example.com/");
        var response = await GetAsync(server, "/does-not-exist");

        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        Assert.Equal("deny", response.Headers[Constants.XFrameOptionsHeaderName]);
    }

    // Returns the host rather than its TestServer, so that each test disposes the whole host.
    private static IHost CreateHost(Action<IApplicationBuilder> pipeline)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .Configure(app =>
                {
                    pipeline(app);
                    app.Run(context => context.Response.WriteAsync("Hello Tests"));
                }))
            .Start();

        host.GetTestServer().BaseAddress = new Uri("https://example.com/");
        return host;
    }

    private static async Task<HttpResponse> GetAsync(TestServer server, string path)
    {
        var context = await server.SendAsync(c =>
        {
            c.Request.Path = path;
            c.Request.Method = HttpMethods.Get;
        }, TestContext.Current.CancellationToken);

        return context.Response;
    }
}
