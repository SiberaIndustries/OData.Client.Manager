#if NETCOREAPP2_2
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TestAuthorizationServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
    }
}
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TestAuthorizationServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
#endif

