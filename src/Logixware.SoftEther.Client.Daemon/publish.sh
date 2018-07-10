#!/bin/bash

# Runtime-Identifier:
# https://docs.microsoft.com/de-de/dotnet/core/rid-catalog

dotnet publish --self-contained --configuration Release --runtime ubuntu.18.04-x64
dotnet publish --self-contained --configuration Release --runtime osx.10.13-x64
