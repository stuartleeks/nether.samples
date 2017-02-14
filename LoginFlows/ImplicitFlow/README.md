# LoginFlows/ImplicitFlow

This sample shows how to use the implicit flow in an HTML/JS application to sign in to nether.

** TODO
* How to configure nether to work with this sample (client, CORS)
* How to build and run the sample


### nether configuration

```powershell

# Set the CORS options for nether

${env:Common:Cors:AllowedOrigins} = "http://localhost:5001"


# Set up the identity client 

${env:Identity:Clients:implicitflow:Name} = "Client for ImplicitFlow sample"
${env:Identity:Clients:implicitflow:AllowedGrantTypes} = "implicit"
${env:Identity:Clients:implicitflow:AllowAccessTokensViaBrowser} = "true"
${env:Identity:Clients:implicitflow:RedirectUris} = "http://localhost:5001/callback.html"
${env:Identity:Clients:implicitflow:PostLogoutRedirectUris}= "http://localhost:5001/index.html"
${env:Identity:Clients:implicitflow:AllowedCorsOrigins} = "http://localhost:5001"
${env:Identity:Clients:implicitflow:ClientSecrets} = "implicitsecret"
${env:Identity:Clients:implicitflow:AllowedScopes} = "openid, profile, nether-all"
```

