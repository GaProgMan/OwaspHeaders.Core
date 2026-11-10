namespace OwaspHeaders.Core.Tests.AgentSkill;

/// <summary>
/// The C# samples from src/AgentSkill/SKILL.md, compiled as part of this project.
/// </summary>
/// <remarks>
/// Each sample sits between a pair of <c>skill-sample</c> markers, and
/// <see cref="SkillDocumentTests"/> fails unless the text between them matches the fenced block
/// of the same name in SKILL.md. Because warnings are errors, a sample which calls an
/// <c>[Obsolete]</c> member fails the build with CS0618, so the skill cannot steer agents
/// towards an API the package is about to remove. See issue #236.
/// </remarks>
internal static class SkillSamples
{
    internal static void HandRolledHeaders(IApplicationBuilder app)
    {
        // <skill-sample:hand-rolled-headers>
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            await next();
        });
        // </skill-sample:hand-rolled-headers>
    }

    internal static void RecommendedDefaults(IApplicationBuilder app)
    {
        // <skill-sample:recommended-defaults>
        app.UseSecureHeadersMiddleware();
        // </skill-sample:recommended-defaults>
    }

    internal static void ConfigureDelegate(IApplicationBuilder app)
    {
        // <skill-sample:configure-delegate>
        app.UseSecureHeadersMiddleware(opt =>
        {
            opt.UseRecommendedDefaults();
            opt.UseHsts(maxAge: 63072000);
            opt.SetUrlsToIgnore(["/health"]);
        });
        // </skill-sample:configure-delegate>
    }

    internal static void SharedConfiguration(IApplicationBuilder app)
    {
        // <skill-sample:shared-configuration>
        static void ConfigureSecureHeaders(SecureHeadersBuilder opt)
        {
            opt.UseRecommendedDefaults();
            opt.SetUrlsToIgnore(["/health"]);
        }

        app.UseSecureHeadersMiddleware(ConfigureSecureHeaders);

        // In a test:
        SecureHeadersBuilder.BuildAndValidate(ConfigureSecureHeaders);
        // </skill-sample:shared-configuration>
    }

    internal static WebApplication PipelinePlacement(WebApplicationBuilder builder)
    {
        // <skill-sample:pipeline-placement>
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseSecureHeadersMiddleware();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        // </skill-sample:pipeline-placement>

        return app;
    }

    internal static void CspAllowOrigin(IApplicationBuilder app)
    {
        // <skill-sample:csp-allow-origin>
        app.UseSecureHeadersMiddleware(opt =>
        {
            opt.UseRecommendedDefaults();
            opt.SetCspUris(
            [
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdn.example.com" }
            ], CspUriType.Script);
        });
        // </skill-sample:csp-allow-origin>
    }
}
