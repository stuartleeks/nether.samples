# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.
param(
    [string] $NetherRootUrl = "http://localhost:5000",
    [string] $ClientId = "higherorlower",
    [string] $ClientSecret = "highlowsecret"
)

# This script expects to be run from the HigherOrLower directory
Push-Location .
try {
    Set-Location .\src
    $env:HIGHLOW_NETHER_ROOT_URL = $NetherRootUrl
    $env:HIGHLOW_CLIENT_ID = $ClientId
    $env:HIGHLOW_CLIENT_SECRET = $ClientSecret
    dotnet run
}
finally {
    Pop-Location
}