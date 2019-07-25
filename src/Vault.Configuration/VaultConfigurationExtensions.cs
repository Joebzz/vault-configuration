using System;
using Microsoft.Extensions.Configuration;

using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;

using Vault.Configuration.SecretManager;

namespace Vault.Configuration
{
    /// <summary>
    /// Extension methods for registering <see cref="VaultConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class VaultConfigurationExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }

            var authInfo = new AppRoleAuthMethodInfo(roleId, secretId);
            return AddVault(configurationBuilder, vaultUri, authInfo, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthMethodInfo authMethodInfo,
            params string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("VaultUri must be a valid URI", nameof(vaultUri)); }
            
            var vaultClientSettings = new VaultClientSettings(vaultUri, authMethodInfo);

            var vaultClient = new VaultClient(vaultClientSettings);

            return AddVault(configurationBuilder, vaultClient, new DefaultVaultSecretManager(), secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IKeyVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            params string[] secretLocationPaths)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (secretLocationPaths == null)
            {
                throw new ArgumentNullException(nameof(secretLocationPaths));
            }
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            configurationBuilder.Add(new VaultConfigurationSource()
            {
                Client = client,
                SecretLocationPaths = secretLocationPaths,
                Manager = manager,
            });

            return configurationBuilder;
        }
    }
}