#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.
NetherRootUrl="http://localhost:5000",
ClientId="higherorlower",
ClientSecret="highlowsecret"

while [[ $# -gt 0 ]]
do
    case "$1" in 
        --nether-root-url)
            NetherRootUrl="$2"
            shift 2
            ;;
        --client-id)
            ClientId="$2"
            shift 2
            ;;
        --client-secret)
            SERVICE_NAME="$2"
            shift 2
            ;;
        *)
            echo "Unexpected '$1'"
            exit 1
            ;;
    esac
done

# This script expects to be run from the HigherOrLower directory
cd src
export HIGHLOW_NETHER_ROOT_URL=$NetherRootUrl
export HIGHLOW_CLIENT_ID=$ClientId
export HIGHLOW_CLIENT_SECRET=$ClientSecret
dotnet run
cd ..