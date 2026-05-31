# ✅ Checklist de Verificação - API GraphQL de Tickets

Use este checklist para verificar se tudo está funcionando corretamente.

---

## 🔧 Pré-requisitos

- [ ] .NET 8 SDK instalado (`dotnet --version`)
- [ ] SQL Server LocalDB instalado
- [ ] Visual Studio Code ou Visual Studio 2022
- [ ] Git (opcional)

### Verificar instalação:
```bash
dotnet --version        # Deve ser 8.0+
sqllocaldb info         # Deve mostrar info do LocalDB
```

---

## 📥 Setup Inicial

- [ ] Pasta `TicketAPI` criada em `f:/Estudos Codigo/Sub/`
- [ ] Todos os arquivos criados (veja [SUMMARY.md](SUMMARY.md))
- [ ] `.gitignore` criado
- [ ] `.env.example` criado

### Verificar arquivos:
```bash
# Na pasta TicketAPI, execute:
ls -la  # Linux/Mac
dir     # Windows
# Deve listar todos os arquivos do projeto
```

---

## 🛠️ Restaurar Dependências

- [ ] `dotnet restore` executado com sucesso
- [ ] Nenhum erro de pacote NuGet
- [ ] `bin/` e `obj/` foram criados

### Executar:
```bash
dotnet restore
# Deve completar sem erros
```

---

## 💾 Banco de Dados

### Entity Framework

- [ ] `dotnet ef` reconhecido como comando
- [ ] DbContext encontrado
- [ ] Connection string configurada em `appsettings.json`

### Verificar:
```bash
dotnet ef dbcontext info
# Deve mostrar: Provider: Microsoft.EntityFrameworkCore.SqlServer
```

### Migrations

- [ ] `dotnet ef database update` executado
- [ ] Banco de dados `TicketAPI` criado
- [ ] Tabela `Tickets` criada com as colunas corretas
- [ ] Seed de dados inseridos (5 tickets)

### Verificar com SQL Server Management Studio:
```sql
USE TicketAPI;
SELECT COUNT(*) FROM Tickets;  -- Deve retornar 5
```

---

## 🚀 Executar Aplicação

- [ ] `dotnet run` executa sem erros
- [ ] Mensagens de inicialização aparecem:
  ```
  🚀 Iniciando API GraphQL de Tickets...
  📊 Disponível em: http://localhost:5000
  ✓ Pronto para receber requisições!
  ```

### Verificar:
```bash
dotnet run
# Deve exibir URL: http://localhost:5000
```

---

## 🌐 Acesso ao GraphQL

- [ ] GraphQL Playground acessível em `http://localhost:5000/graphql`
- [ ] Página carrega sem erros 404
- [ ] Banana Cake Pop interface exibida
- [ ] Campo de editor de queries visível
- [ ] Botão "Play" disponível

### Testar:
1. Abra navegador
2. Vá para: `http://localhost:5000/graphql`
3. Deve carregar o Banana Cake Pop

---

## 📊 Testar Queries (Leitura)

### Query 1: Buscar Todos
```graphql
query {
  tickets {
    id
    protocolo
    nomeCliente
  }
}
```
- [ ] Retorna 5 tickets de seed
- [ ] Sem erros GraphQL
- [ ] JSON válido

### Query 2: Buscar por ID
```graphql
query {
  ticketPorId(id: 1) {
    protocolo
    nomeCliente
  }
}
```
- [ ] Retorna ticket com ID 1
- [ ] Dados corretos

### Query 3: Filtrar por Status
```graphql
query {
  ticketsPorStatus(status: ABERTO) {
    id
    protocolo
  }
}
```
- [ ] Retorna apenas tickets ABERTO
- [ ] Sem erros

### Query 4: Contar Total
```graphql
query {
  totalTickets
}
```
- [ ] Retorna valor numérico
- [ ] Valor é 5 (seed inicial)

---

## ✏️ Testar Mutations (Escrita)

### Mutation 1: Criar Ticket Não-Crítico
```graphql
mutation {
  criarTicket(input: {
    protocolo: "TEST-001"
    nomeCliente: "Teste Usuario"
    tipo: CONTA
    severidade: MEDIA
  }) {
    id
    protocolo
    status
    dataCriacao
  }
}
```
- [ ] Ticket criado com sucesso
- [ ] Status é "ABERTO"
- [ ] ID gerado automaticamente
- [ ] Retorna dados completos

### Mutation 2: Criar Ticket Crítico
```graphql
mutation {
  criarTicket(input: {
    protocolo: "CRITICO-001"
    nomeCliente: "Teste Critico"
    tipo: FRAUDE
    severidade: CRITICA
  }) {
    id
    protocolo
    status
  }
}
```
- [ ] Ticket critico criado
- [ ] Sem erros na execução
- [ ] **Evento publicado** (próximo passo)

### Mutation 3: Atualizar Status
```graphql
mutation {
  atualizarStatusTicket(id: 1, novoStatus: EMANALISE) {
    id
    status
  }
}
```
- [ ] Status atualizado
- [ ] Retorna novo status

### Mutation 4: Deletar Ticket
```graphql
mutation {
  deletarTicket(id: 7)
}
```
- [ ] Retorna true/false
- [ ] Ticket removido do banco

---

## 🔔 Testar Subscriptions (Tempo Real)

### Setup para Teste

1. **Aba 1 do Navegador - Subscription:**
```graphql
subscription {
  ticketCriticoCriado {
    id
    protocolo
    nomeCliente
    severidade
  }
}
```
- [ ] Click "Start"
- [ ] Status: "Waiting for server events..."

2. **Aba 2 do Navegador - Mutation:**
```graphql
mutation {
  criarTicket(input: {
    protocolo: "SUB-TEST-001"
    nomeCliente: "Teste Subscription"
    tipo: PIX
    severidade: CRITICA
  }) {
    id
  }
}
```
- [ ] Execute mutation

3. **Volta para Aba 1:**
- [ ] 🔔 **Notificação apareceu!**
- [ ] Dados do ticket crítico visível
- [ ] Sem delay de mais de 2 segundos

### ✅ Subscription Funcionando:
- [ ] Múltiplas notificações chegam
- [ ] WebSocket conectado
- [ ] Dados corretos

---

## 🗄️ Validar Banco de Dados

### Com SQL Server Management Studio (SSMS)

```sql
-- Verificar banco
USE TicketAPI;

-- Verificar tabela existe
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'Tickets';

-- Contar registros
SELECT COUNT(*) FROM Tickets;

-- Ver seed data
SELECT * FROM Tickets ORDER BY DataCriacao DESC;

-- Verificar índices
EXEC sp_helpindex 'Tickets';

-- Verificar estrutura
EXEC sp_help 'Tickets';
```

✅ **Esperado:**
- [ ] 1 tabela `Tickets`
- [ ] 8+ registros (5 seed + criados no teste)
- [ ] 4+ índices (PK, UX Protocolo, IX Status/Severidade, IX DataCriacao)
- [ ] Todos os tipos de dados corretos

---

## 📚 Validar Documentação

- [ ] `README.md` existe e é legível
- [ ] `QUICK_START.md` existe
- [ ] `EXEMPLOS_GRAPHQL.md` existe com exemplos
- [ ] `ARQUITETURA.md` existe com diagramas
- [ ] `DOCKER.md` existe
- [ ] `SUMMARY.md` existe
- [ ] `INDEX.md` existe
- [ ] `FLUXO_SUBSCRIPTION.md` existe

### Verificar conteúdo:
```bash
# Verificar tamanho de cada arquivo
wc -l *.md  # Linux/Mac
Get-Item *.md | Select-Object Length  # Windows
```

---

## 🐳 Validar Docker (Opcional)

```bash
# Verificar se Docker está instalado
docker --version
docker-compose --version

# Build imagem
docker build -t ticketapi:test .
# [ ] Build completa sem erros

# Executar com docker-compose
docker-compose up -d
# [ ] SQL Server container inicia
# [ ] TicketAPI container inicia

# Verificar logs
docker-compose logs -f
# [ ] Aplicação iniciou corretamente

# Testar
curl http://localhost:5000/graphql
# [ ] Retorna HTML do Banana Cake Pop

# Parar
docker-compose down
```

---

## 🎯 Performance

### Verificar Índices
```sql
-- Índices criados?
SELECT name, type_desc 
FROM sys.indexes 
WHERE object_id = OBJECT_ID('Tickets')
ORDER BY name;
```
- [ ] 5+ índices presentes
- [ ] Índice único em Protocolo
- [ ] Índice composto em Status+Severidade

### Query Speed
```sql
-- Deve ser < 100ms
SET STATISTICS TIME ON
SELECT * FROM Tickets WHERE Status = 'ABERTO'
SET STATISTICS TIME OFF
```
- [ ] Tempo de execução < 100ms
- [ ] Sem table scans (deve usar índice)

---

## 🔐 Validar Segurança

- [ ] CORS configurado em `Program.cs`
- [ ] `AllowedOrigins` está definido
- [ ] Não há credenciais em `.env` versionado
- [ ] `.gitignore` ignora arquivos sensíveis
- [ ] Conexão string não está em logs

### Verificar Program.cs:
```csharp
// [ ] Deve ter:
app.UseCors("AllowAll");  // ou política específica
```

---

## 📝 Validar Código

### Code Review Checklist

#### Models
- [ ] `Ticket.cs` - Propriedades corretas
- [ ] `Enums.cs` - 3 enums definidos
- [ ] XML comments em classes

#### Data
- [ ] `TicketDbContext.cs` - DbContext configurado
- [ ] Fluent API usada para config
- [ ] Índices definidos
- [ ] Seed implementado

#### GraphQL
- [ ] `Query.cs` - 10+ métodos
- [ ] `Mutation.cs` - 4 métodos
- [ ] `Subscription.cs` - 2 métodos
- [ ] Types definidos
- [ ] Input types definidos

#### Program.cs
- [ ] Hot Chocolate registrado
- [ ] DbContext adicionado
- [ ] Migrations executadas
- [ ] Seed executado
- [ ] CORS configurado
- [ ] Endpoints mapeados

---

## 🎓 Validar Aprendizado

Você consegue responder?

- [ ] O que é GraphQL?
- [ ] Qual a diferença entre Query e Mutation?
- [ ] O que é uma Subscription?
- [ ] Como funcionam subscriptions com WebSocket?
- [ ] O que é Entity Framework Core?
- [ ] Qual a função dos índices?
- [ ] O que é Hot Chocolate?
- [ ] Como o Pub/Sub funciona?

---

## ⏱️ Tempo Total de Verificação

| Etapa | Tempo |
|-------|-------|
| Setup | 5 min |
| Queries | 5 min |
| Mutations | 5 min |
| Subscriptions | 10 min |
| Banco de Dados | 5 min |
| Docker (opcional) | 10 min |
| **TOTAL** | **40 min** |

---

## 🆘 Problemas Comuns

### ❌ "Database doesn't exist"
- [ ] Executou `dotnet ef database update`?
- [ ] SQL Server LocalDB está rodando?
- [ ] Connection string está correta?

### ❌ "Port 5000 already in use"
- [ ] Feche outras aplicações na porta 5000
- [ ] Use porta diferente: `dotnet run --urls http://localhost:5001`

### ❌ "GraphQL endpoint not found (404)"
- [ ] Aplicação está rodando (`dotnet run`)?
- [ ] URL correta? `http://localhost:5000/graphql`
- [ ] Browser cache limpo?

### ❌ "Subscription não funciona"
- [ ] Está em 2 abas diferentes?
- [ ] Clicou "Start" na subscription?
- [ ] Mutations são críticas (`severidade: CRITICA`)?

---

## 📊 Status Final

Após completar todos os checkboxes acima, você terá:

```
✅ Projeto configurado corretamente
✅ Banco de dados funcional
✅ API GraphQL rodando
✅ Queries funcionando
✅ Mutations funcionando
✅ Subscriptions em tempo real
✅ Documentação completa
✅ Exemplos prontos
✅ Docker funcional (opcional)
✅ Segurança básica
```

---

## 🎯 Próximos Passos

1. **Explorar o código:**
   - Entender cada arquivo
   - Modificar queries
   - Adicionar validações

2. **Expandir funcionalidades:**
   - Adicionar autenticação
   - Implementar paginação
   - Adicionar filtros avançados

3. **Deploy:**
   - Usar Docker em produção
   - Configurar CI/CD
   - Implementar monitoramento

4. **Aprender mais:**
   - Ler documentação completa
   - Estudar código-fonte
   - Explorar Hot Chocolate docs

---

## 📞 Suporte

Se não conseguir completar um item:

1. Consulte [README.md](README.md) - Documentação completa
2. Veja [TROUBLESHOOTING](README.md#troubleshooting) - Soluções comuns
3. Confira [INDEX.md](INDEX.md) - Navegação de documentos
4. Leia logs de erro cuidadosamente

---

## ✨ Parabéns!

Se completou todos os checkboxes, você tem uma **API GraphQL profissional em produção**! 🎉

**Desenvolvido com ❤️ usando ASP.NET Core 8, Hot Chocolate e Entity Framework Core**
