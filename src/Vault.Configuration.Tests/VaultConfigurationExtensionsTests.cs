using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods;

using Vault.Configuration.SecretManager;
using VaultSharp.V1.AuthMethods.AppRole;

namespace Vault.Configuration.Tests
{
    [TestClass]
    public class VaultConfigurationExtensionsTests
    {
        private const string _vaultConfigurationUrl = "http://127.0.0.1:8200";
        private const string _vaultConfigurationRoleId = "fake_vault_role_id";
        private const string _vaultConfigurationSecretId = "fake_vault_secret_id";
        private const string _vaultConfigurationMountPoint = "fake_vault_mount_point";
        private readonly string[] _vaultConfigurationSecretPath = new string [] { "fake_vault_secret_path1", "fake_vault_secret_path2" };

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAppRole_ThrowsArgumentException_ForEmptyUrl()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    "",
                    _vaultConfigurationRoleId,
                    _vaultConfigurationSecretId,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAppRole_ThrowsArgumentException_ForEmptyRoleId()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    _vaultConfigurationUrl,
                    "",
                    _vaultConfigurationSecretId,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAppRole_ThrowsArgumentException_ForEmptySecretId()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    _vaultConfigurationUrl,
                    _vaultConfigurationRoleId,
                    "",
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void AddVaultWithAppRole_ThrowsUriFormatException_ForEmptyUrl()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    "test",
                    _vaultConfigurationRoleId,
                    _vaultConfigurationSecretId,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAppRole_ThrowsArgumentException_ForEmptyMountPoint()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    _vaultConfigurationUrl,
                    _vaultConfigurationRoleId,
                    _vaultConfigurationSecretId,
                    "",
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        public void AddVaultWithAppRole_AddsSource()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAppRole(
                    _vaultConfigurationUrl,
                    _vaultConfigurationRoleId,
                    _vaultConfigurationSecretId,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsNotNull(configuration.Sources);
            Assert.AreEqual(1, configuration.Sources.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAuthMethodInfo_ThrowsArgumentException_ForEmptyMountPoint()
        {
            // Arrange
            var mockAuthMethodInfo = new Mock<IAuthMethodInfo>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAuthMethodInfo(
                    "",
                    mockAuthMethodInfo.Object,
                    "",
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithAuthMethodInfo_ThrowsArgumentException_ForEmptyUrl()
        {
            // Arrange
            var mockAuthMethodInfo = new Mock<IAuthMethodInfo>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAuthMethodInfo(
                    "",
                    mockAuthMethodInfo.Object,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void AddVaultWithAuthMethodInfo_ThrowsUriFormatException_ForEmptyUrl()
        {
            // Arrange
            var mockAuthMethodInfo = new Mock<IAuthMethodInfo>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAuthMethodInfo(
                    "test",
                    mockAuthMethodInfo.Object,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        public void AddVaultWithAuthMethodInfo_AddsSource()
        {
            // Arrange
            var authMethodInfo = new AppRoleAuthMethodInfo(_vaultConfigurationRoleId, _vaultConfigurationSecretId);
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithAuthMethodInfo(
                    _vaultConfigurationUrl,
                    authMethodInfo,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsNotNull(configuration.Sources);
            Assert.AreEqual(1, configuration.Sources.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddVault_ThrowsArgumentNullException_ForNullManager()
        {
            // Arrange
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVault(
                    mockVaultClient.Object,
                    null,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddVault_ThrowsArgumentNullException_ForNullClient()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVault(
                    null,
                    mockVaultSecretManager.Object,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddVault_ThrowsArgumentNullException_ForNullMountPoint()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVault(
                    mockVaultClient.Object,
                    mockVaultSecretManager.Object,
                    null,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddVault_ThrowsArgumentNullException_ForNullSecretPaths()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVault(
                    mockVaultClient.Object,
                    mockVaultSecretManager.Object,
                    _vaultConfigurationMountPoint,
                    null
                );

            // Assert - Expects Exception
        }


        [TestMethod]
        public void AddVault_AddsSource()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVault(
                    mockVaultClient.Object,
                    mockVaultSecretManager.Object,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsNotNull(configuration.Sources);
            Assert.AreEqual(1, configuration.Sources.Count);
        }
    }
}