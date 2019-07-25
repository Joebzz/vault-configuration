using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using VaultSharp;
using VaultSharp.V1.Commons;
using VaultSharp.Core;

using Vault.Configuration.SecretManager;

namespace Vault.Configuration
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {
        private readonly IVaultClient _client;
        private IEnumerable<string> _secretPaths;
        private readonly IVaultSecretManager _manager;

        /// <summary>
        /// Creates a new instance of <see cref="VaultConfigurationProvider"/>.
        /// </summary>
        /// <param name="client">The <see cref="IVaultClient"/> to use for retrieving values.</param>
        /// <param name="secretPath">The secret path to read</param>
        /// <param name="manager"></param>
        public VaultConfigurationProvider(IVaultClient client, IVaultSecretManager manager, IEnumerable<string> secretPaths)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _secretPaths = secretPaths ?? throw new ArgumentNullException(nameof(secretPaths));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public override void Load() => LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var secretPath in _secretPaths)
            {
                try
                {
                    var kv2Secret = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath);
                    var secretData = kv2Secret.Data;
                    Dictionary<string, object> secretDictionary = secretData.Data;

                    AddSecrets(data, secretData, secretDictionary);
                }
                catch (VaultApiException vaultApiException)
                {
                    Console.WriteLine("Vault API Error:" + vaultApiException.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.ToString());
                }
            }

            Data = data;
        }

        private void AddSecrets(Dictionary<string, string> data, SecretData secretData, IDictionary<string, object> secretDictionary)
        {
            foreach (var kvp in secretDictionary)
            {
                // if the manager cannot load this key value pair just skip it
                if (!_manager.Load(secretData, kvp.Key))
                {
                    continue;
                }
                
                var key = _manager.GetKey(secretData, kvp.Key);
                data.Add(key, kvp.Value?.ToString());
            }
        }
    }
}