using OwaspHeaders.Core.Enums;
using Scalar.AspNetCore;
// the OwaspHeaders.Core.Enums using statement is required for example 4,
// which is commented out by default. As such, Rider (and linting tools)
// will likely complain about the fact that it is unnecessary.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure logging - this shows how to see SecureHeaders logs in console output
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Set to Debug to see all SecureHeaders logs

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Example 1: Basic SecureHeaders with default logging (uses default Event IDs 1000-3999)
var listOfUrlsToIgnore = new List<string> { "/skipthis", "/scalar/v1" };
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseRecommendedDefaults();
    opt.SetUrlsToIgnore(listOfUrlsToIgnore);
});

// Example 2 (commented): Custom configuration with custom Event IDs
// This demonstrates how to configure SecureHeaders with custom Event IDs to avoid conflicts
/*
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseHsts();
    opt.UseXFrameOptions();
    opt.UseContentTypeOptions();
    opt.UseReferrerPolicy();
    opt.WithLoggingEventIdBase(5000); // Use Event IDs starting from 5000 instead of 1000
    opt.SetUrlsToIgnore(["/skipthis"]);
});
*/

// Example 3 (commented): Fully custom Event ID configuration
// This shows complete control over individual Event IDs
/*
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseHsts();
    opt.UseXFrameOptions();
    opt.UseContentTypeOptions();
    opt.SetUrlsToIgnore(["/skipthis"]);
    opt.WithLoggingEventIds(new SecureHeadersLoggingConfiguration
    {
        MiddlewareInitialized = new EventId(9001, "SecureHeadersInit"),
        HeadersAdded = new EventId(9002, "HeadersSet"),
        RequestIgnored = new EventId(9003, "RequestSkipped"),
        ConfigurationError = new EventId(9999, "ConfigError")
    });
});
*/

// Example 4 (commented): Clear-Site-Data configuration for logout endpoints
// This demonstrates path-specific Clear-Site-Data header configuration for enhanced logout security
/*
app.UseSecureHeadersMiddleware(opt =>
{
    opt.UseHsts();
    opt.UseXFrameOptions();
    opt.UseContentTypeOptions();
    opt.UseReferrerPolicy();
    // Clear all data on standard logout
    opt.AddClearSiteDataPath("/auth/logout", ClearSiteDataOptions.wildcard);
    // Selective clearing for API logout
    opt.AddClearSiteDataPath("/auth/api/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies);
    // Maximum security for admin logout
    opt.AddClearSiteDataPath("/auth/admin/logout", ClearSiteDataOptions.wildcard);
    opt.SetUrlsToIgnore(["/skipthis"]);
});
*/

// Example 5 (commented): Bare minimum required to use the new, experimental ReportingEndpointsPolicy header
/*
var reportingEndpoints =
    new Dictionary<string, Uri> {
        { "standard", new Uri("https://localhost:5000/reporting-endpoint") }
    };
app.UseSecureHeadersMiddleware(opt => opt.UseReportingEndpointsPolicy(reportingEndpoints));
*/

// Example 6 (commented): Sharing one configuration between Program.cs and a test
// Hoisting the delegate into a named method means a test can assert the configuration is valid
// via SecureHeadersBuilder.BuildAndValidate(ConfigureSecureHeaders), without standing up a host.
/*
static void ConfigureSecureHeaders(SecureHeadersBuilder opt)
{
    opt.UseRecommendedDefaults();
    opt.SetUrlsToIgnore(["/skipthis"]);
}

app.UseSecureHeadersMiddleware(ConfigureSecureHeaders);
*/

app.MapControllers();

app.Run();
