# LoginFlows/FacebookJavascript

This sample shows how to use the Facebook JavaScript SDK in a browser application to sign in to nether.

** TODO
* How to configure nether to work with this sample (client, CORS)
* Set up FB app and configure the sample
* How to build and run the sample

### sample configuration


```powershell
# Set the AppID for your facebook app here: see https://developers.facebook.com/apps
${env:FacebookAppId} = "<insert your app id here>"

```

### nether configuration

```powershell

# Set the Facebook AppToken for the fb-usertoken custom grant flow
# See https://developers.facebook.com/tools/accesstoken for the access token once you have created your app

${env:Identity:SignInMethods:FacebookUserAccessToken:Enabled}="true" 
${env:Identity:SignInMethods:FacebookUserAccessToken:AppToken}="<insert your app token here>" 


# Set the CORS options for nether

${env:Common:Cors:AllowedOrigins} = "http://localhost:5002"


# Set up the identity client 

${env:Identity:Clients:fb_js:Name} = "Client for Facebook JavaScript SDK sample"
${env:Identity:Clients:fb_js:AllowedGrantTypes} = "fb-usertoken"
#${env:Identity:Clients:fb_js:AllowAccessTokensViaBrowser} = "true"
#${env:Identity:Clients:fb_js:RedirectUris} = "http://localhost:5002/callback.html"
#${env:Identity:Clients:fb_js:PostLogoutRedirectUris}= "http://localhost:5001/index.html"
${env:Identity:Clients:fb_js:AllowedCorsOrigins} = "http://localhost:5002"
${env:Identity:Clients:fb_js:ClientSecrets} = "fbjssecret"
${env:Identity:Clients:fb_js:AllowedScopes} = "openid, profile, nether-all"

```

