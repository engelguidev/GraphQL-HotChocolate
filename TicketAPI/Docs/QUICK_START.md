# 🚀 Guia Rápido de Inicialização

## ⚡ Quick Start (5 minutos)

### Pré-requisitos
- ✅ .NET 8 SDK instalado ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- ✅ SQL Server LocalDB ([Incluído no Visual Studio](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb))

### Passo 1: Restaurar Dependências
```bash
cd TicketAPI
dotnet restore
```

### Passo 2: Criar Banco de Dados
```bash
dotnet ef database update
```

### Passo 3: Executar a Aplicação
```bash
dotnet run
```

Você verá:
```
🚀 Iniciando API GraphQL de Tickets...
📊 Disponível em: http://localhost:5000
🔍 GraphQL Playground: http://localhost:5000/graphql
✓ Pronto para receber requisições!
```

### Passo 4: Acessar o GraphQL Playground
Abra no navegador: **http://localhost:5000/graphql**

---

## 🧪 Teste Rápido

### 1. Executar uma Query
Cole no Playground e clique "Play":

```graphql
query {
  tickets {
    id
    protocolo
    nomeCliente
    severidade
  }
}
```

### 2. Executar uma Mutation
Cole e clique "Play":

```graphql
mutation {
  criarTicket(input: {
    protocolo: "TEST-001"
    nomeCliente: "Seu Nome"
    tipo: PIX
    severidade: CRITICA
  }) {
    id
    protocolo
    status
  }
}
```

### 3. Testar Subscription (Tempo Real!)

**Aba 1 - Subscription:**
```graphql
subscription {
  ticketCriticoCriado {
    protocolo
    nomeCliente
    severidade
  }
}
```
- Cole no Playground
- Clique "Start"
- Verá: "Waiting for server events..."

**Aba 2 - Mutation:**
```graphql
mutation {
  criarTicket(input: {
    protocolo: "TEST-002"
    nomeCliente: "Teste Real Time"
    tipo: FRAUDE
    severidade: CRITICA
  }) {
    id
  }
}
```
- Abra outra aba do Playground
- Execute esta mutation

**Resultado:**
- Volte para Aba 1 da subscription
- Você verá a notificação em tempo real! 🎉

---

## 📁 Estrutura do Projeto

```
TicketAPI/
├── Models/              # Entidades e Enums
├── Data/                # DbContext e Configurações
├── GraphQL/
│   ├── Queries/         # Operações de leitura
│   ├── Mutations/       # Operações de escrita
│   ├── Subscriptions/   # Eventos em tempo real
│   └── Types/           # Tipos GraphQL
├── Program.cs           # Configuração principal
├── appsettings.json     # Configurações
└── README.md            # Documentação completa
```

---

## 🔧 Problemas Comuns

### ❌ "Connection to SQL Server failed"
**Solução:**
```bash
# Verificar LocalDB
sqllocaldb info mssqllocaldb

# Se não existir, criar
sqllocaldb create mssqllocaldb
```

### ❌ "Table 'Tickets' doesn't exist"
**Solução:**
```bash
dotnet ef database update
```

### ❌ "Port 5000 already in use"
**Solução:**
```bash
# Usar outra porta
dotnet run --urls http://localhost:5001
```

### ❌ Subscription não funciona
**Solução:**
- Abre 2 abas diferentes do Playground
- Certifique-se de clicar "Start" na subscription
- Verifique o console para erros

---

## 📚 Arquivos Importantes

| Arquivo | Descrição |
|---------|-----------|
| `README.md` | Documentação completa (✨ **Leia primeiro!**) |
| `EXEMPLOS_GRAPHQL.md` | 20+ exemplos prontos para copiar/colar |
| `Program.cs` | Configuração do Hot Chocolate e EF Core |
| `Data/TicketDbContext.cs` | Definição do banco de dados |
| `Models/Ticket.cs` | Entidade principal |

---

## 🌐 URLs Úteis

- **GraphQL Playground**: http://localhost:5000/graphql
- **Status da API**: http://localhost:5000/
- **Documentação Local**: README.md

---

## 📝 Próximos Passos

1. ✅ Leia [README.md](README.md) para documentação completa
2. ✅ Veja [EXEMPLOS_GRAPHQL.md](EXEMPLOS_GRAPHQL.md) para queries prontas
3. ✅ Explore o Banana Cake Pop Playground
4. ✅ Teste as subscriptions em tempo real
5. ✅ Integre com sua aplicação frontend

---

## 🎯 Objetivo da Subscription

A principal feature é a **subscription em tempo real** para tickets críticos:

```
┌────────────┐         ┌──────────────────┐         ┌────────────────┐
│  Cliente 1 │ ◄─────► │  Hot Chocolate   │ ◄─────► │  SQL Server    │
│(Subscribe) │         │  (WebSocket)     │         │  (Banco)       │
└────────────┘         └──────────────────┘         └────────────────┘
                                │
                                │ Publica evento
                                │
                       ┌────────▼────────┐
                       │  In-Memory Pub/ │
                       │     Sub System   │
                       └────────┬────────┘
                                │
                    ┌───────────┴───────────┐
                    │                       │
                    ▼                       ▼
            ┌──────────────┐        ┌──────────────┐
            │  Cliente 2   │        │  Cliente 3   │
            │  (Recebe em  │        │  (Recebe em  │
            │ tempo real!) │        │ tempo real!) │
            └──────────────┘        └──────────────┘
```

---

## 💡 Fluxo Típico de Uso

1. **Monitor abre o Playground**
   - Cria uma subscription para `ticketCriticoCriado`
   - Clica "Start" e fica aguardando eventos

2. **Cliente entra no banco com problema crítico**
   - Cria um ticket via mutation `criarTicket`
   - Com `severidade: CRITICA`

3. **Sistema publica evento automaticamente**
   - Salva no BD
   - Publica no `ITopicEventSender`

4. **Monitor recebe notificação em tempo real**
   - Via WebSocket (não precisa dar refresh!)
   - Vê os detalhes do ticket
   - Pode tomar ação imediata

---

## 🎓 Aprendizado

Este projeto demonstra:

- ✅ **GraphQL** com Hot Chocolate
- ✅ **Entity Framework Core** com SQL Server
- ✅ **Subscriptions** em tempo real (WebSocket)
- ✅ **Injeção de Dependências** (ASP.NET Core)
- ✅ **Boas práticas** de estrutura de projeto
- ✅ **Async/Await** em C#
- ✅ **Validação** e tratamento de erros

---

Qualquer dúvida, consulte a [Documentação Completa](README.md)! 📖

**Happy Coding! 🚀**
