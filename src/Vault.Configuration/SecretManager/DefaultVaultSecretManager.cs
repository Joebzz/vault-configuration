using VaultSharp.V1.Commons;

namespace Vault.Configuration.SecretManager
{
    /// <summary>
    /// Default instance of the <see cref="IVaultSecretManager"/> 
    /// </summary>
    public class DefaultVaultSecretManager: IVaultSecretManager
    {
        public string GetKey(SecretData secretData, string secretKey) => secretKey;

        public bool Load(SecretData secretData, string key) => true;
    }
}