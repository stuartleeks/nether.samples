$gameRoot = "http://localhost:8080"

# Set CORS options for main API
${env:Common:Cors:AllowedOrigins} = $gameRoot

# Create a new client id for connecting to identity service
${env:Identity:Clients:higherorlower:AllowedGrantTypes} = "guest-access"
${env:Identity:Clients:higherorlower:ClientSecrets} = "highlowsecret"
${env:Identity:Clients:higherorlower:AllowedScopes} = "nether-all"
${env:Identity:Clients:higherorlower:AllowedCorsOrigins} = $gameRoot


