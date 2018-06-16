using Dna;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Add Microsoft.Extensions.DependencyInjection
            //     Microsoft.Extensions.Configuration(for ConfigurationBuilder)
            //     Microsoft.Extensions.Configuration.Json(for AddJsonFile)

            //
            //   So its your job to distribute the service collection to any add-in,
            //   library or part of your code that needs to add its own dependecies
            //
            //   If using configuration you should also pass that along
            //
            //   Then once done you build the service collection into a service provider
            //   which is now your source of dependency injection where all of your code
            //   can get services from the provider.GetService<>
            //
            //   So typically this provider is a static instance in a core library
            //   so all of your code can access it
            //

            // Create a new list of dependencies
            var services = new ServiceCollection();

            // At this point, all dependencies can be added to the DI system via the service collection

            // Configurations are used heavily in the .Net Core DI for configuring services
            // So we can make use of that 
            // Create our configuration sources
            var configurationBuilder = new ConfigurationBuilder();
            // Add environment variables
            //.AddEnvironmentVariables();

            // Add application settings json files
            configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // Build configuration
            var configuration = configurationBuilder.Build();

            // Inject configuration into services
            services.AddSingleton<IConfiguration>(configuration);

            // Build provider
            var provider = services.BuildServiceProvider();

            // After this point, DI is available and through the provider


            // 
            //    Dna framework 
            //
            //
            // Use Framework.Construct<FrameworkConstruction> for totally blank service provider
            // containing just the FrameworkEnvironment service, no Configuration or anything else

            // Use the DefaultFrameworkConstruction to add a Configuration similar to ASP.Net 
            // with the Configuration source of appsettings.json file, and to also add
            // a basic console/debug logger and an exception handler that just logs errors
            Framework.Construct<DefaultFrameworkConstruction>()
                // Add further services like this
                .AddFileLogger()
                // And once done build
                .Build();

            // Now the service provider is here
            Framework.Provider.GetService<ILogger>().LogCriticalSource("Some important message");

            // Or shortcuts here
            FrameworkDI.Logger.LogCriticalSource("Shortcut to important message");
        }
    }
}
