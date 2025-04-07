#!/bin/sh
set -e

echo "Applying database migrations..."
dotnet ef database update --project SelfEduNet/SelfEduNet.csproj --startup-project SelfEduNet/SelfEduNet.csproj

echo "Starting the application..."
exec dotnet SelfEduNet.dll