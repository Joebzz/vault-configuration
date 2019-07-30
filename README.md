# DotNet Core Configuration Extension for Hashicorp Vault 

## Configuring Vault Dev Server to run the sample

### App Role Auth
The following guide describes how to configure Vault in dev mode to use for the samples [Console](/samples/console) & [Web](/samples/web):

1. Start the server in dev mode: 

```bash
vault server -dev -dev-root-token-id="root"
```

Verify the status

```bash
set VAULT_ADDR=http://127.0.0.1:8200
vault status
```

2. Create a secret that we'll retrieve in the sample

```bash
vault kv put secret/sampleapp/config my-value=test1 my-value-2=test2 my-value-3=test3
```

3. Enable approle auth method 

```bash
vault auth enable approle
```

4. Create a policy giving access to the secrets required by the sample app loading from an .hcl file (example sampleapp-pol.hcl in the project vault-configs folder):

```bash
vault policy write sampleapp sampleapp-pol.hcl
```

```bash
vault write auth/approle/role/sampleapp-role policies="sampleapp"
```

1. Get the RoleId for the role

```bash
vault read auth/approle/role/sampleapp-role/role-id
```
This will generate a UUID:

```bash
Key        Value
---        -----
role_id    6cfff770-173b-6ae6-0aed-2dee23d00f36
```

7. Generate a new Secret ID for the role

```bash
vault write -f auth/approle/role/sampleapp-role/secret-id
```

This will generate a SecretId and a SecretID Accessor:

```bash
Key                   Value
---                   -----
secret_id             01547bf5-72e9-39d0-0917-64794b834cac
secret_id_accessor    f860260f-bf25-f427-bbe3-6539935759fa
```

8. Make the RoleId and SecretId available to the app as configuration values. You should inject these into your app in a secure way, for example using environment variables. NOTE: In the samples in this project it just uses the appsettings.json file but this is not secure.

### Certificate Auth
To use Certificate Auth you will need to run the server using https in production mode [deployment guide] (https://learn.hashicorp.com/vault/getting-started/deploy). For the below instructions you will need to install [openssl](https://www.xolphin.com/support/OpenSSL/OpenSSL_-_Installation_under_Windows) and add it to your PATH environment variable.

1. Create a TLS Cert using Openssl, (example req.conf file in vault-coinfigs folder)
 ```bash
 openssl req -x509 -nodes -days 730 -newkey rsa:2048 -keyout vault.key -out vault.crt -config req.conf
 ```

2. Start the server in production mode (example config.hcl in the project vault-configs folder): 

```bash
vault server -config=config.hcl
```

Verify the status

```bash
set VAULT_ADDR=https://localhost:8200
vault status
```

3. Initialise the production server 

5. Generate a PFX certificate file if you dont have one already. Follow the instructions here https://www.sslsupportdesk.com/export-ssl-certificate-private-key-pfx-using-mmc-windows/ for creating it in windows 10.

6. Use openssl to create a PEM certificate to import into Vault.

The openssl command to run to create the public key cert to be imported into the vault is: 
```bash
openssl pkcs12 -in vault-test-cert.pfx -clcerts -nokeys -out vault-test-cert.pem
```

6. Importing the cert into the vault

Enable Certificate Auth
```bash
vault auth enable cert
```

7. Add the generated cert to the Vault and add it to the sampleapp policy.
```bash
vault write auth/cert/certs/web display_name=sampleapp policies=sampleapp certificate=@vault-test-cert.pem
```

NOTE: Certificate path in appsettings file points to the pfx certificate