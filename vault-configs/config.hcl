ui = true
disable_mlock = true

storage "file" {
  path = "data"
}

listener "tcp" {
 address     = "127.0.0.1:8200"
 tls_disable = false
 tls_cert_file = "cert/vault.crt"
 tls_key_file = "cert/vault.key"
}

api_addr = "https://localhost:8200"