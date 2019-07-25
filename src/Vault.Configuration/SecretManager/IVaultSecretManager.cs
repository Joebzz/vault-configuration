using System.Collections.Generic;

using VaultSharp.V1.Commons;

namespace Vault.Configuration.SecretManager
{
    /// <summary>
    /// The <see cref="IVaultSecretManager"/> instance used to control secret loading.
    /// </summary>
    public interface IVaultSecretManager
    {
        /// <summary>
        /// Checks if the value for <paramref name="key"/> should be used.
        /// </summary>
        /// <param name="secretData">The <see cref="SecretData"/> instance.</param>
        /// <param name="key">The key in the <see cref="Secret{TData}"/>.</param>
        /// <returns><code>true</code> is secrets value should be loaded, otherwise <code>false</code>.</returns>
        bool Load(SecretData secretData, string key);

        /// <summary>
        /// Maps secret key to a configuration key.
        /// </summary>
        /// <param name="secretData">The <see cref="SecretData"/> instance.</param>
        /// <param name="secretKey">The secret key instance.</param>
        /// <returns>Configuration key name to store secret value.</returns>
        string GetKey(SecretData secretData, string secretKey);
    }
}