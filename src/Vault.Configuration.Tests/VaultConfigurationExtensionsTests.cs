using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;

using Vault.Configuration.SecretManager;

namespace Vault.Configuration.Tests
{
    [TestClass]
    public class VaultConfigurationExtensionsTests
    {
        private const string _vaultConfigurationUrl = "http://127.0.0.1:8200";
        private const string _vaultConfigurationMountPoint = "fake_vault_mount_point";
        private readonly string[] _vaultConfigurationSecretPath = new string[] { "fake_vault_secret_path1", "fake_vault_secret_path2" };

        private const string _vaultConfigurationRoleId = "fake_vault_role_id";
        private const string _vaultConfigurationSecretId = "fake_vault_secret_id";

        private readonly string _vaultCertPath = Directory.GetCurrentDirectory() + "/test.pem";
        private const string _vaultCertPassword = "test";

        #region Configuration Options
        [TestMethod]
        public void AddVaultWithConfigurationOptions_Disabled_DoesntAddSource()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var vaultConfigOptions = new VaultConfigurationOptions
            {
                Enabled = false
            };

            // Act
            configurationBuilder.AddVaultWithConfigurationOptions(vaultConfigOptions);

            // Assert
            Assert.IsNotNull(configurationBuilder);
            Assert.IsNotNull(configurationBuilder.Sources);
            Assert.AreEqual(0, configurationBuilder.Sources.Count);
        }

        [TestMethod]
        public void AddVaultWithConfigurationOptions_EnabledWithAppRole_AddsSource()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var vaultConfigOptions = new VaultConfigurationOptions
            {
                Enabled = true,
                Url = _vaultConfigurationUrl,
                SecretMountPoint = _vaultConfigurationMountPoint,
                SecretLocationPaths = _vaultConfigurationSecretPath,
                RoleId = _vaultConfigurationRoleId,
                SecretId = _vaultConfigurationSecretId
            };

            // Act
            configurationBuilder.AddVaultWithConfigurationOptions(vaultConfigOptions);

            // Assert
            Assert.IsNotNull(configurationBuilder);
            Assert.IsNotNull(configurationBuilder.Sources);
            Assert.AreEqual(1, configurationBuilder.Sources.Count);
        }
        
        [TestMethod]
        public void AddVaultWithConfigurationOptions_EnabledWithCert_AddsSource()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();
            var vaultConfigOptions = new VaultConfigurationOptions
            {
                Enabled = true,
                Url = _vaultConfigurationUrl,
                SecretMountPoint = _vaultConfigurationMountPoint,
                SecretLocationPaths = _vaultConfigurationSecretPath,
                CertificatePassword = _vaultCertPassword,
                CertificatePath = _vaultCertPath
            };

            // Act
            configurationBuilder.AddVaultWithConfigurationOptions(vaultConfigOptions);

            // Assert
            Assert.IsNotNull(configurationBuilder);
            Assert.IsNotNull(configurationBuilder.Sources);
            Assert.AreEqual(1, configurationBuilder.Sources.Count);
        }
        #endregion Configuration Options

        #region App Role Authentication
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

        #endregion App Role Authentication

        #region Certificate Authentication
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithCert_ThrowsArgumentException_ForEmptyUrl()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithCert(
                    "",
                    _vaultCertPath,
                    _vaultCertPassword,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithCert_ThrowsArgumentException_ForEmptyCertPath()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithCert(
                    _vaultConfigurationUrl,
                    "",
                    _vaultCertPassword,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithCert_ThrowsArgumentException_ForEmptyCertPassword()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithCert(
                    _vaultConfigurationUrl,
                    _vaultCertPath,
                    "",
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddVaultWithCert_ThrowsArgumentException_ForEmptyMountPoint()
        {
            // Arrange
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithCert(
                    _vaultConfigurationUrl,
                    _vaultCertPath,
                    _vaultCertPassword,
                    "",
                    _vaultConfigurationSecretPath
                );

            // Assert - Expects Exception
        }

        [TestMethod]
        public void AddVaultWithCert_AddsSource()
        {
            // Arrange
            var mockVaultSecretManager = new Mock<IVaultSecretManager>();
            var mockVaultClient = new Mock<IVaultClient>();
            var configuration = new ConfigurationBuilder();

            // Act
            configuration.AddVaultWithCert(
                    _vaultConfigurationUrl,
                    _vaultCertPath,
                    _vaultCertPassword,
                    _vaultConfigurationMountPoint,
                    _vaultConfigurationSecretPath
                );

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsNotNull(configuration.Sources);
            Assert.AreEqual(1, configuration.Sources.Count);
        }

        #endregion Certificate Authentication

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