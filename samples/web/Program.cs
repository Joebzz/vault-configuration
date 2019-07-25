using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Vault.Configuration.Sample.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AddCustomConfiguration)
                .UseStartup<Startup>();

        private static void AddCustomConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            
            // Get the Vault Section from the configuration
            var vaultSection = configuration.GetSection("Vault");
            var vaultConfiguration = vaultSection.Get<VaultConfigurationOptions>(); // Convert it to a strongly typed object
            if (vaultConfiguration != null && vaultConfiguration.Enabled)
            {
                builder.AddVaultWithAppRole(
                    vaultConfiguration.Url,
                    vaultConfiguration.RoleId,
                    vaultConfiguration.SecretId,
                    vaultConfiguration.SecretPath
                );
            }
        }
    }
}
