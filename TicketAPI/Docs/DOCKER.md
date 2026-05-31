# 🐳 Guia de Docker e Deployment

## Usando Docker Compose (Recomendado)

### Pré-requisitos
- Docker Desktop instalado ([Download](https://www.docker.com/products/docker-desktop))
- Pelo menos 4GB de RAM disponível

### ⚡ Quick Start com Docker

```bash
# 1. Na pasta do projeto
cd TicketAPI

# 2. Construir e iniciar containers
docker-compose up -d

# 3. Verificar status
docker-compose ps

# 4. Ver logs
docker-compose logs -f ticketapi

# 5. Acessar GraphQL
# Abra no navegador: http://localhost:5000/graphql

# 6. Para parar
docker-compose down

# 7. Para limpar tudo (incluindo volume do BD)
docker-compose down -v
```

### Saída esperada:

```
STATUS
ticketapi-sqlserver   Up (healthy)
ticketapi-app         Up
```

---

## Rodando Manualmente com Docker

### 1. Build da Imagem

```bash
docker build -t ticketapi:latest .
```

### 2. Executar SQL Server em Container

```bash
docker run -e "ACCEPT_EULA=Y" \
           -e "SA_PASSWORD=YourPassword123!" \
           -p 1433:1433 \
           -d \
           --name ticketapi-db \
           mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Executar TicketAPI

```bash
docker run -p 5000:5000 \
           -e "ConnectionStrings__DefaultConnection=Server=ticketapi-db;Database=TicketAPI;User Id=sa;Password=YourPassword123!;Encrypt=False;" \
           --link ticketapi-db \
           -d \
           --name ticketapi \
           ticketapi:latest
```

### 4. Verificar Logs

```bash
docker logs -f ticketapi
```

---

## Verificação de Saúde

### Health Check da API

```bash
curl http://localhost:5000/
```

Resposta esperada:
```json
{
  "message": "API GraphQL de Tickets - Bem-vindo!",
  "graphql": "Acesse http://localhost:5000/graphql",
  "version": "1.0.0"
}
```

### Verificar Container

```bash
# Ver containers rodando
docker ps

# Ver logs de erro
docker logs ticketapi-app

# Executar comando dentro do container
docker exec -it ticketapi-app dotnet --version

# Verificar processo
docker top ticketapi-app
```

---

## Conectar ao SQL Server no Container

### Via SQL Server Management Studio (SSMS)

```
Server: localhost,1433
Login: sa
Password: YourPassword123!
```

### Via SqlCmd

```bash
sqlcmd -S localhost,1433 -U sa -P "YourPassword123!" -Q "SELECT @@VERSION"
```

### Via Azure Data Studio

```
Connection type: Microsoft SQL Server
Server: localhost,1433
Authentication type: SQL Login
Username: sa
Password: YourPassword123!
```

---

## Variáveis de Ambiente

Configure no `docker-compose.yml`:

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  ConnectionStrings__DefaultConnection: "Server=sqlserver,1433;Database=TicketAPI;User Id=sa;Password=YourPassword123!;Encrypt=False;"
  Logging__LogLevel__Default: Information
```

---

## Network do Docker

O `docker-compose.yml` cria uma rede interna:

```
┌─────────────────────────────────────────┐
│  Docker Network: ticketapi-network     │
│                                         │
│  ┌──────────────┐   ┌──────────────┐  │
│  │ sqlserver    │───│ ticketapi    │  │
│  │ 1433         │   │ 5000         │  │
│  └──────────────┘   └──────────────┘  │
│       (container)         (container)  │
└─────────────────────────────────────────┘
```

---

## Troubleshooting

### ❌ "Container already exists"

```bash
docker-compose down
docker-compose up -d
```

### ❌ "Port 5000 already in use"

```bash
# Mudar porta no docker-compose.yml:
# ports:
#   - "5001:5000"  ← mude para 5001

docker-compose up -d
```

### ❌ "Connection refused to SQL Server"

```bash
# Verifique se SQL Server está healthy
docker-compose ps

# Se não estiver ready, aguarde mais um momento
sleep 10
docker-compose up -d
```

### ❌ "Out of memory"

Aumente a memória disponível para Docker:
- Docker Desktop → Settings → Resources → Memory (ajuste para 4GB+)

### ❌ Resetar tudo

```bash
# Parar e remover tudo
docker-compose down -v

# Limpar imagens não usadas
docker system prune -a

# Recomeçar
docker-compose up -d
```

---

## Deployment em Produção

### 1. Usar Variáveis de Ambiente Seguras

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  ConnectionStrings__DefaultConnection: ${DB_CONNECTION_STRING}
  # Não hardcode passwords!
```

### 2. Usar .env file

Crie arquivo `.env` (não versionado em Git):

```
DB_CONNECTION_STRING=Server=prod-sql.example.com;Database=TicketAPI;User Id=produser;Password=SecurePassword123!;
```

Reference no docker-compose.yml:
```yaml
env_file: .env
```

### 3. Persistência de Dados

```yaml
volumes:
  - sqlserver_data:/var/opt/mssql
  - ./backups:/var/opt/mssql/backup
```

### 4. Limites de Recursos

```yaml
services:
  ticketapi:
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M
```

### 5. Restart Policy

```yaml
restart: unless-stopped
```

---

## Monitoramento

### Logs em Tempo Real

```bash
docker-compose logs -f ticketapi
```

### Estatísticas de Uso

```bash
docker stats ticketapi-app
```

### Inspecionar Container

```bash
docker inspect ticketapi-app
```

---

## CI/CD com Docker

### GitHub Actions Example

```yaml
name: Build and Push

on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Build Image
        run: docker build -t ticketapi:${{ github.sha }} .
      
      - name: Push to Registry
        run: |
          echo ${{ secrets.DOCKER_PASSWORD }} | docker login -u ${{ secrets.DOCKER_USERNAME }} --password-stdin
          docker tag ticketapi:${{ github.sha }} myregistry/ticketapi:latest
          docker push myregistry/ticketapi:latest
```

---

## Performance Tips

1. **Use BuildKit** para builds mais rápidos:
   ```bash
   docker buildx build -t ticketapi:latest .
   ```

2. **Multi-stage builds** (já configurado no Dockerfile):
   - Reduz tamanho da imagem final
   - Imagem final: ~200MB

3. **Cache de camadas**:
   ```bash
   docker build --cache-from ticketapi:latest -t ticketapi:latest .
   ```

4. **Comprimir imagem**:
   ```bash
   docker save ticketapi:latest | gzip > ticketapi.tar.gz
   ```

---

## Links Úteis

- [Docker Documentation](https://docs.docker.com/)
- [Docker Hub - .NET Images](https://hub.docker.com/_/microsoft-dotnet)
- [SQL Server Docker Images](https://hub.docker.com/_/microsoft-mssql-server)

---

**Happy Containerizing! 🐳**
