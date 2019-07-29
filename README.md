# DotNet Core Configuration Extension for Hashicorp Vault 

### Configuring Vault Dev Server to run the sample

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

4. Create a policy giving access to the secrets required by the sample app

Either loading from an .hcl file (example sampleapp-pol.hcl in the project root):

```bash
vault policy write sampleapp sampleapp-pol.hcl
```

```bash
vault write auth/approle/role/sampleapp-role policies="sampleapp"
```

6. Get the RoleId for the role

```bash
vault read auth/approle/role/sampleapp-role/role-id
```
This will generate a UUID:

```bash
Key        Value
---        -----
role_id    e5b1091d-09ae-916c-5e65-a1b13f2b4696
```

7. Generate a new Secret ID for the role

```bash
vault write -f auth/approle/role/sampleapp-role/secret-id
```

This will generate a SecretId and a SecretID Accessor:

```bash
Key                   Value
---                   -----
secret_id             1efc430a-9018-4ac8-4953-833f067e0063
secret_id_accessor    01bd4cdf-d223-8c1c-fefb-8196171179bc
```

8. Make the RoleId and SecretId available to the app as configuration values. You should inject these into your app in a secure way, for example using environment variables. NOTE: In the samples in this project it just uses the appsettings.json file but this is not secure.
