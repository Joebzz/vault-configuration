using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Cert;

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
        /// Uses the Vault Options object to decide which Authenitcation method to use for the Vault
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultConfigurationOptions">The Options from the initial Configuration</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithConfigurationOptions(
            this IConfigurationBuilder configurationBuilder,
            VaultConfigurationOptions vaultConfigurationOptions)
        {
            // If the vault is disable just return the configuration builder without adding any further config providers
            if(vaultConfigurationOptions == null || !vaultConfigurationOptions.Enabled)
                return configurationBuilder;

            // If the Certificate Path is set use this otherwise try and use app role
            if (!string.IsNullOrEmpty(vaultConfigurationOptions.CertificatePath))
            {
                return AddVaultWithCert(
                    configurationBuilder,
                    vaultConfigurationOptions.Url,
                    vaultConfigurationOptions.CertificatePath,
                    vaultConfigurationOptions.CertificatePassword,
                    vaultConfigurationOptions.SecretMountPoint,
                    vaultConfigurationOptions.SecretLocationPaths
                );
            }
            else
            {
                return AddVaultWithAppRole(
                    configurationBuilder,
                    vaultConfigurationOptions.Url,
                    vaultConfigurationOptions.RoleId,
                    vaultConfigurationOptions.SecretId,
                    vaultConfigurationOptions.SecretMountPoint,
                    vaultConfigurationOptions.SecretLocationPaths
                );
            }
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretMountPoint">The main path for the secrets to load from.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithAppRole(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string roleId,
            string secretId,
            string secretMountPoint,
            string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentException("roleId must not be null or empty", nameof(roleId)); }
            if (string.IsNullOrEmpty(secretId)) { throw new ArgumentException("secretId must not be null or empty", nameof(secretId)); }
            if (string.IsNullOrEmpty(secretMountPoint)) { throw new ArgumentException("secretMountPoint must not be null or empty", nameof(secretMountPoint)); }

            var authMethodInfo = new AppRoleAuthMethodInfo(roleId, secretId);
            return AddVaultWithAuthMethodInfo(configurationBuilder, vaultUri, authMethodInfo, secretMountPoint, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="certificatePath">The path for the cert to use for authentication.</param>
        /// <param name="certificatePassword">The password to use for the cert authentication.</param>
        /// <param name="secretMountPoint">The main path for the secrets to load from.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithCert(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            string certificatePath,
            string certificatePassword,
            string secretMountPoint,
            string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(certificatePath)) { throw new ArgumentException("certificatePath must not be null or empty", nameof(certificatePath)); }
            if (string.IsNullOrEmpty(certificatePassword)) { throw new ArgumentException("certificatePassword must not be null or empty", nameof(certificatePassword)); }
            if (string.IsNullOrEmpty(secretMountPoint)) { throw new ArgumentException("secretMountPoint must not be null or empty", nameof(secretMountPoint)); }

            var clientCertificate = new X509Certificate2(certificatePath, certificatePassword);
            var authMethodInfo = new CertAuthMethodInfo(clientCertificate);

            return AddVaultWithAuthMethodInfo(configurationBuilder, vaultUri, authMethodInfo, secretMountPoint, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="vaultUri">The Vault uri with port.</param>
        /// <param name="roleId">The AppRole role_id to use for authentication.</param>
        /// <param name="secretId">The secret_id to use for authentication.</param>
        /// <param name="secretMountPoint">The main path for the secrets to load from.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVaultWithAuthMethodInfo(
            this IConfigurationBuilder configurationBuilder,
            string vaultUri,
            IAuthMethodInfo authMethodInfo,
            string secretMountPoint,
            string[] secretLocationPaths)
        {
            if (string.IsNullOrWhiteSpace(vaultUri)) { throw new ArgumentException("vaultUri must be a valid URI", nameof(vaultUri)); }
            if (string.IsNullOrEmpty(secretMountPoint)) { throw new ArgumentException("secretMountPoint must not be null or empty", nameof(secretMountPoint)); }
            if (!Uri.IsWellFormedUriString(vaultUri, UriKind.RelativeOrAbsolute)) { throw new UriFormatException("vaultUri must be a valid URI"); }

            var vaultClientSettings = new VaultClientSettings(vaultUri, authMethodInfo);

            var vaultClient = new VaultClient(vaultClientSettings);

            return AddVault(configurationBuilder, vaultClient, new DefaultVaultSecretManager(), secretMountPoint, secretLocationPaths);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from Hashicorp Vault.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="manager">The <see cref="IKeyVaultSecretManager"/> instance used to control secret loading.</param>
        /// <param name="secretMountPoint">The main path for the secrets to load from.</param>
        /// <param name="secretLocationPaths">The paths for the secrets to load.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVault(
            this IConfigurationBuilder configurationBuilder,
            IVaultClient client,
            IVaultSecretManager manager,
            string secretMountPoint,
            string[] secretLocationPaths)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (secretMountPoint == null)
            {
                throw new ArgumentNullException(nameof(secretMountPoint));
            }
            if (secretLocationPaths == null)
            {
                throw new ArgumentNullException(nameof(secretLocationPaths));
            }
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var vaultConfigSource = new VaultConfigurationSource()
            {
                Client = client,
                SecretMountPoint = secretMountPoint,
                SecretLocationPaths = secretLocationPaths,
                Manager = manager,
            };

            configurationBuilder.Add(vaultConfigSource);
            return configurationBuilder;
        }
    }
}