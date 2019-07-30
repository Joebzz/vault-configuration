using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Vault.Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = configBuilder.Build();
            // Get the Vault Section from the configuration
            var vaultSection = config.GetSection("Vault");
            var vaultConfigurationOptions = vaultSection.Get<VaultConfigurationOptions>(); // Convert it to a strongly typed object
            configBuilder.AddVaultWithConfigurationOptions(vaultConfigurationOptions);
            config = configBuilder.Build(); // Rebuild the configuration

            // Demonstrate how the values are retrieved from the config 
            var testValue1 = config["my-value"];
            var testValue2 = config["my-value-2"];
            var testValue3 = config["my-value-3"];
            Console.WriteLine("my-value : " + testValue1);
            Console.WriteLine("my-value-2 : " + testValue2);
            Console.WriteLine("my-value-3 : " + testValue3);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
