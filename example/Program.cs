using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSecureHeadersMiddleware(
    SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration()
        .UseContentSecurityPolicy()
        .SetCspUris(
            new List<ContentSecurityPolicyElement>()
            {
                new ContentSecurityPolicyElement() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement()
                {
                    CommandType = CspCommandType.Uri,
                    DirectiveOrUri =
                        "https://api.github.com https://*.githubusercontent.com https://*.google-analytics.com https://owaspadmin.azurewebsites.net https://*.twimg.com https://platform.twitter.com https://www.youtube.com https://*.doubleclick.net"
                }
            }, CspUriType.DefaultUri)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>()
            {
                new ContentSecurityPolicyElement() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new ContentSecurityPolicyElement()
                {
                    CommandType = CspCommandType.Uri,
                    DirectiveOrUri =
                        "https://api.github.com https://*.githubusercontent.com https://*.google-analytics.com https://owaspadmin.azurewebsites.net https://*.twimg.com https://platform.twitter.com https://www.youtube.com https://*.doubleclick.net"
                }
            }, CspUriType.FrameAncestors)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>()
            {
                new ContentSecurityPolicyElement()
                {
                    CommandType = CspCommandType.Uri,
                    DirectiveOrUri =
                        "https://*.vuejs.org https://*.stripe.com https://*.wufoo.com https://*.sched.com https://*.google.com https://*.twitter.com https://www.youtube.com https://w.soundcloud.com"
                }
            }, CspUriType.Frame)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>
            {
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-eval" },
                new()
                {
                    CommandType = CspCommandType.Uri,
                    DirectiveOrUri =
                        "https://fonts.googleapis.com https://app.diagrams.net https://cdnjs.cloudflare.com https://cse.google.com https://*.vuejs.org https://*.stripe.com https://*.wufoo.com https://*.youtube.com https://*.meetup.com https://*.sched.com https://*.google-analytics.com https://unpkg.com https://buttons.github.io https://www.google.com https://*.gstatic.com https://*.twitter.com https://*.twimg.com"
                }
            }, CspUriType.Script)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>
            {
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                new()
                {
                    CommandType = CspCommandType.Uri,
                    DirectiveOrUri =
                        "https://*.gstatic.com https://cdnjs.cloudflare.com https://www.google.com https://fonts.googleapis.com https://platform.twitter.com https://*.twimg.com"
                },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" },
            }, CspUriType.Style)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>
            {
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "fonts.gstatic.com" }
            }, CspUriType.Font)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>
            {
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://pay.google.com" }
            }, CspUriType.Manifest)
        .SetCspUris(
            new List<ContentSecurityPolicyElement>
            {
                new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.globalappsec.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "www.w3.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://licensebuttons.net" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://img.shields.io" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twitter.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://github.githubassets.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://platform.twitter.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.githubusercontent.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.vercel.app" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.cloudfront.net" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.coreinfrastructure.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.securityknowledgeframework.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://badges.gitter.im" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://travis-ci.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://api.travis-ci.org" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://s3.amazonaws.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://snyk.io" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://coveralls.io" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://requires.io" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://github.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.googleapis.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google.com" },
                new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.gstatic.com" },
            }, CspUriType.Img)
);

app.MapControllers();

app.Run();
