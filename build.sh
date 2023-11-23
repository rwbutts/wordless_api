#!/bin/sh
echo dotnet build -c Release WordlessAPI.csproj -o ./deploy
dotnet build -c Release WordlessAPI.csproj -o ./deploy
