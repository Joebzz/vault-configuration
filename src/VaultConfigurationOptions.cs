namespace HashiCorp.Vault.DotNetCore.Configuration
{
    public class VaultConfigurationOptions
    {
        public bool Enabled { get; set; }
        public string  Url { get; set; }
        public string RoleId { get; set; }
        public string SecretId { get; set; }
        public string SecretPath { get; set; }
    }
}