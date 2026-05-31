# Dockerfile para TicketAPI
# Build multi-stage para otimizar tamanho da imagem

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

# Copiar arquivo de projeto
COPY ["TicketAPI.csproj", "."]

# Restaurar dependências
RUN dotnet restore "TicketAPI.csproj"

# Copiar código-fonte
COPY . .

# Build da aplicação
RUN dotnet build "TicketAPI.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "TicketAPI.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar curl para health check
RUN apt-get update && apt-get install -y curl --no-cache && rm -rf /var/lib/apt/lists/*

# Copiar arquivo publicado do stage anterior
COPY --from=build /app/publish .

# Expor porta
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Executar aplicação
ENTRYPOINT ["dotnet", "TicketAPI.dll"]
