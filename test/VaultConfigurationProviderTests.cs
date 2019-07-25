using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using VaultSharp;
using VaultSharp.V1.Commons;

using HashiCorp.Vault.DotNetCore.Configuration.SecretManager;

namespace HashiCorp.Vault.DotNetCore.Configuration.Tests
{
    [TestClass]
    public class VaultConfigurationProviderTests
    {
        private const string _secretPath = "/secrets/Development/testapp";

        [TestMethod]
        public void LoadsAllKeysFromVault()
        {
            // Arrange
            var mockClient = new Mock<IVaultClient>();
            var secret1Id = "Secret1";
            var secret2Id = "Secret2";
            var secret1ValueIn = "Value1";
            var secret2ValueIn = "Value2";

            var fakeReturnSecretData = new Secret<SecretData>
            {
                Data = new SecretData
                {
                    Data = new Dictionary<string, object> {
                            { secret1Id, secret1ValueIn },
                            { secret2Id, secret2ValueIn }
                        }
                }
            };

            mockClient.Setup(c => c.V1.Secrets.KeyValue.V2.ReadSecretAsync(_secretPath, null, "kv", null))
                .ReturnsAsync(fakeReturnSecretData);

            // Act
            var provider = new VaultConfigurationProvider(mockClient.Object, new DefaultVaultSecretManager(), new[] { _secretPath });
            provider.Load();

            // Assert
            mockClient.VerifyAll();

            var secret1ValueOut = "";
            var secret2ValueOut = "";
            provider.TryGet(secret1Id, out secret1ValueOut);
            provider.TryGet(secret2Id, out secret2ValueOut);

            Assert.AreEqual(secret1ValueIn, secret1ValueOut);
            Assert.AreEqual(secret2ValueIn, secret2ValueOut);
        }

        [TestMethod]
        public void SupportsColonInSecretKeys()
        {
            // Arrange
            var mockClient = new Mock<IVaultClient>();
            var secret1Id = "Section:Secret1";
            var secret1ValueIn = "Value1";
            var secret2Id = "Section:Secret2";
            var secret2ValueIn = "Value2";

            var fakeReturnSecretData = new Secret<SecretData>
            {
                Data = new SecretData
                {
                    Data = new Dictionary<string, object> {
                            { secret1Id, secret1ValueIn },
                            { secret2Id, secret2ValueIn }
                        }
                }
            };

            mockClient.Setup(c => c.V1.Secrets.KeyValue.V2.ReadSecretAsync(_secretPath, null, "kv", null))
                .ReturnsAsync(fakeReturnSecretData);

            // Act
            var provider = new VaultConfigurationProvider(mockClient.Object, new DefaultVaultSecretManager(), new[] { _secretPath });
            provider.Load();

            // Assert
            mockClient.VerifyAll();

            var secret1ValueOut = "";
            var secret2ValueOut = "";
            provider.TryGet(secret1Id, out secret1ValueOut);
            provider.TryGet(secret2Id, out secret2ValueOut);

            Assert.AreEqual(secret1ValueIn, secret1ValueOut);
            Assert.AreEqual(secret2ValueIn, secret2ValueOut);
        }

        [TestMethod]
        public void SupportsReload()
        {
            // Arrange
            var mockClient = new Mock<IVaultClient>();
            var secret1Id = "Section:Secret1";
            var secret1ValueIn = "Value1";

            mockClient.Setup(c => c.V1.Secrets.KeyValue.V2.ReadSecretAsync(_secretPath, null, "kv", null))
                .Returns((string path, int version, string mountPoint, string wrapTimeToLive) =>
                {
                    return Task.FromResult(
                        new Secret<SecretData>
                        {
                            Data = new SecretData
                            {
                                Data = new Dictionary<string, object> {
                                    { secret1Id, secret1ValueIn }
                                }
                            }
                        }
                    );
                });

            // Act
            var provider = new VaultConfigurationProvider(mockClient.Object, new DefaultVaultSecretManager(), new[] { _secretPath });
            provider.Load();

            // Assert
            mockClient.VerifyAll();

            var secret1ValueOut = "";
            provider.TryGet(secret1Id, out secret1ValueOut);
            Assert.AreEqual(secret1ValueIn, secret1ValueOut);

            secret1ValueIn = "Value1Updated";
            provider.Load();
            provider.TryGet(secret1Id, out secret1ValueOut);
            Assert.AreEqual(secret1ValueIn, secret1ValueOut);
        }
    }
}
