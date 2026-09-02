# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de solução
COPY ["ERP.SaaS.sln", "./"]
COPY ["src/ERP.Shared/ERP.Shared.csproj", "./src/ERP.Shared/"]
COPY ["src/ERP.Master/ERP.Master.csproj", "./src/ERP.Master/"]
COPY ["src/ERP.Tenant/ERP.Tenant.csproj", "./src/ERP.Tenant/"]
COPY ["src/ERP.Web/ERP.Web.csproj", "./src/ERP.Web/"]

# Restaurar dependências
RUN dotnet restore "ERP.SaaS.sln"

# Copiar o resto do código
COPY . .
WORKDIR "/src/src/ERP.Web"

# Build
RUN dotnet build "ERP.Web.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "ERP.Web.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ERP.Web.dll"]