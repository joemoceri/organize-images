using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizeImages;

namespace OrganizeImages
{
    public class Program 
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<App>();
            
            app!.Run();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // add options
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration.GetSection("Settings"));

            // add app and services
            services.AddTransient<App>();
            services.AddTransient<IImageOrganizer, ImageOrganizer>();
        }
    }
}