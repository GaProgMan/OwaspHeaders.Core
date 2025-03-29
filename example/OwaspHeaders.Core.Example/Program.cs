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

// Example of using both a relative URL where headers should not be preset and the [EXPERIMENTAL] Reporting-Endpoints header
var listOfUrlsToIgnore = new List<string> { "/skipthis" };
var reportingEndpoints =
    new Dictionary<string, Uri> {
        { "standard", new Uri("https://localhost:5000/reporting-endpoint") }
    };
var secureHeadersMiddlewareConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
    .UseReportingEndpointsPolicy(reportingEndpoints).SetUrlsToIgnore(listOfUrlsToIgnore).Build();
app.UseSecureHeadersMiddleware(secureHeadersMiddlewareConfig);

app.MapControllers();

app.Run();
