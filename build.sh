#!/bin/sh
echo dotnet build -c Release WordlessAPI.csproj  -r win-x64 --no-self-contained -o ./deploy
dotnet build -c Release WordlessAPI.csproj  -r win-x64 --no-self-contained -o ./deploy


