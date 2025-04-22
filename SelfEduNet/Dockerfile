FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SelfEduNet/SelfEduNet.csproj", "SelfEduNet/"]
RUN dotnet restore "SelfEduNet/SelfEduNet.csproj"
COPY . .
WORKDIR "/src/SelfEduNet"
RUN dotnet build "SelfEduNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfEduNet.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SelfEduNet.dll"]