using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;


namespace example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            // Add functionality to inject IOptions<T>, we'll need this for our middleware
            services.AddOptions();

            // Add our SecureHeadersMiddlewareConfiguration object so it can be injected
            services.Configure<SecureHeadersMiddlewareConfiguration>(
                Configuration.GetSection("SecureHeadersMiddlewareConfiguration"));
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IOptions<SecureHeadersMiddlewareConfiguration> secureHeaderSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSecureHeadersMiddleware(secureHeaderSettings.Value);
            app.UseMvc();
        }
    }
}
