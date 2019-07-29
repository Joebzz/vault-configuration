# Login with AppRole
path "auth/approle/login" {
  capabilities = [ "create", "read" ]
}

# Read test data (v2)
path "secret/data/sampleapp/*" {
  capabilities =  [ "read" ]
}
