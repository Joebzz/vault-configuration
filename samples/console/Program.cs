using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Vault.Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var config = configBuilder.Build();
            // Get the Vault Section from the configuration
            var vaultSection = config.GetSection("Vault");
            var vaultConfiguration = vaultSection.Get<VaultConfigurationOptions>(); // Convert it to a strongly typed object
            if (vaultConfiguration != null && vaultConfiguration.Enabled)
            {
                configBuilder.AddVaultWithAppRole(
                    vaultConfiguration.Url,
                    vaultConfiguration.RoleId,
                    vaultConfiguration.SecretId,
                    vaultConfiguration.SecretPath
                );
            }
            
            var testValue1 = config["my-value"];
            var testValue2 = config["my-value-2"];
            var testValue3 = config["my-value-3"];
            Console.WriteLine(testValue1);
            Console.WriteLine(testValue2);
            Console.WriteLine(testValue3);
        }
    }
}
