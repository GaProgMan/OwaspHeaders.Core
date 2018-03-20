using ClacksMiddleware.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;


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
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.GnuTerryPratchett();
            app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration());
            app.UseMvc();
        }
    }
}
