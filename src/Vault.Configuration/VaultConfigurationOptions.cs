namespace Vault.Configuration
{
    public class VaultConfigurationOptions
    {
        public bool Enabled { get; set; }
        public string  Url { get; set; }
        public string RoleId { get; set; }
        public string SecretId { get; set; }
        public string SecretMountPoint { get; set; }
        public string[] SecretLocationPaths { get; set; }

        // For Certificate Auth
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}