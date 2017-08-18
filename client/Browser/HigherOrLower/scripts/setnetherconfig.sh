#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

gameRoot="http://localhost:8080"

# Set CORS options for main API
export Common__Cors__AllowedOrigins=$gameRoot

# Create a new client id for connecting to identity service
export Identity__Clients__higherorlower__AllowedGrantTypes="guest-access"
export Identity__Clients__higherorlower__ClientSecrets="highlowsecret"
export Identity__Clients__higherorlower__AllowedScopes="nether-all"
export Identity__Clients__higherorlower__AllowedCorsOrigins=$gameRoot


