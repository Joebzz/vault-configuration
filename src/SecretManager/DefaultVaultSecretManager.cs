using VaultSharp.V1.Commons;

namespace HashiCorp.Vault.DotNetCore.Configuration.SecretManager
{
    /// <summary>
    /// Default instance of the <see cref="IVaultSecretManager"/> 
    /// </summary>
    public class DefaultVaultSecretManager: IVaultSecretManager
    {
        public string GetKey(SecretData secretData, string secretKey) => secretKey;

        public virtual bool Load(SecretData secretData, string key) => true;
    }
}