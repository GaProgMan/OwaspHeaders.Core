using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace example
{
    public class Program
    {
        protected static readonly List<string> LocalConfigFileNames = new List<string>()
        {
            "secureHeaderSettings.json"
        };
        
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("secureHeaderSettings.json", optional: true, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
