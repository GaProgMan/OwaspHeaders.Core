using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace example
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
