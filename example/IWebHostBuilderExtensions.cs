using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace example
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder AddJsonConfigFiles(this IWebHostBuilder webHostBuilder, IEnumerable<string> jsonConfigFileNames)
        {
            foreach (var jsonConfig in jsonConfigFileNames)
            {
                webHostBuilder.ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile(jsonConfig, optional: true, reloadOnChange: true);
                });
            }

            return webHostBuilder;
        }
    }
}