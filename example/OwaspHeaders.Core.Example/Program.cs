var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging - this shows how to see SecureHeaders logs in console output
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Set to Debug to see all SecureHeaders logs

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Example 1: Basic SecureHeaders with default logging (uses default Event IDs 1000-3999)
var listOfUrlsToIgnore = new List<string> { "/skipthis" };
app.UseSecureHeadersMiddleware(urlIgnoreList: listOfUrlsToIgnore);

// Example 2 (commented): Custom configuration with custom Event IDs
// This demonstrates how to configure SecureHeaders with custom Event IDs to avoid conflicts
/*
var customConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .UseReferrerPolicy()
    .WithLoggingEventIdBase(5000) // Use Event IDs starting from 5000 instead of 1000
    .SetUrlsToIgnore(["/skipthis"])
    .Build();

app.UseSecureHeadersMiddleware(customConfig);
*/

// Example 3 (commented): Fully custom Event ID configuration
// This shows complete control over individual Event IDs
/*
var fullyCustomLoggingConfig = new SecureHeadersLoggingConfiguration
{
    MiddlewareInitialized = new EventId(9001, "SecureHeadersInit"),
    HeadersAdded = new EventId(9002, "HeadersSet"),
    RequestIgnored = new EventId(9003, "RequestSkipped"),
    ConfigurationError = new EventId(9999, "ConfigError")
};

var fullyCustomConfig = SecureHeadersMiddlewareBuilder
    .CreateBuilder()
    .UseHsts()
    .UseXFrameOptions()
    .UseContentTypeOptions()
    .WithLoggingEventIds(fullyCustomLoggingConfig)
    .SetUrlsToIgnore(["/skipthis"])
    .Build();

app.UseSecureHeadersMiddleware(fullyCustomConfig);
*/

app.MapControllers();

app.Run();
