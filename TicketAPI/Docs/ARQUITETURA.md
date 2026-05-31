# 🏗️ Arquitetura da API GraphQL de Tickets

## Diagrama de Componentes

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        🖥️  CLIENTE (Navegador)                              │
│                                                                             │
│   ┌──────────────────────────────────────────────────────────────────┐    │
│   │  GraphQL Banana Cake Pop (IDE GraphQL)                           │    │
│   │  - Queries (Leitura)                                             │    │
│   │  - Mutations (Escrita)                                           │    │
│   │  - Subscriptions (WebSocket - Tempo Real)                        │    │
│   └──────────────────┬───────────────────────────────────────────────┘    │
└────────────────────┼─────────────────────────────────────────────────────┘
                     │
                     │ HTTP/HTTPS & WebSocket
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                    🚀 ASP.NET CORE 8 SERVER                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                   🔥 HOT CHOCOLATE (GraphQL Server)                 │   │
│  │                                                                      │   │
│  │  ┌────────────────────┐  ┌──────────────────┐  ┌──────────────┐   │   │
│  │  │  QUERY TYPE        │  │  MUTATION TYPE   │  │ SUBSCRIPTION │   │   │
│  │  │                    │  │                  │  │  TYPE        │   │   │
│  │  │ • GetTickets       │  │ • CriarTicket    │  │              │   │   │
│  │  │ • GetTicketPorId   │  │ • AtualizarSt... │  │ • TicketCr...│   │   │
│  │  │ • GetTicketsPor... │  │ • AtualizarSe... │  │   Criado     │   │   │
│  │  │ • GetTotalTickets  │  │ • DeletarTicket  │  │              │   │   │
│  │  │                    │  │                  │  │ • TicketCr...│   │   │
│  │  └────────┬───────────┘  └────────┬─────────┘  │   iado       │   │   │
│  │           │                       │             └────────┬─────┘   │   │
│  │           └───────────────────────┼─────────────────────┘         │   │
│  │                                   │                               │   │
│  │                          ┌────────▼────────┐                      │   │
│  │                          │  DbContext      │                      │   │
│  │                          │  Injeção de     │                      │   │
│  │                          │  Dependências   │                      │   │
│  │                          └────────┬────────┘                      │   │
│  └───────────────────────────────────┼──────────────────────────────┘   │
│                                      │                                    │
│  ┌───────────────────────────────────▼──────────────────────────────┐   │
│  │         📦 IN-MEMORY PUB/SUB SYSTEM (Subscriptions)             │   │
│  │                                                                  │   │
│  │  ITopicEventSender  ──► TopicBuffer ──► Subscriptions          │   │
│  │  (Publicador)           (Fila)         (WebSocket Listeners)   │   │
│  │                                                                  │   │
│  │  Evento: "TicketCriticoCriado" ──► Notifica todos os clientes  │   │
│  └──────────────────────────────────────────────────────────────────┘   │
│                                                                              │
└────────────────┬─────────────────────────────────────────────────────────────┘
                 │
                 │ Entity Framework Core
                 │ (Connection String)
                 │
                 ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│                    🗄️  SQL SERVER DATABASE                                   │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ DATABASE: TicketAPI                                                 │  │
│  │                                                                      │  │
│  │ ┌──────────────────────────────────────────────────────────────┐   │  │
│  │ │ TABLE: Tickets                                              │   │  │
│  │ ├──────────────────────────────────────────────────────────────┤   │  │
│  │ │ Id (PK)                      │ Int                           │   │  │
│  │ │ Protocolo (UNIQUE INDEX)     │ VARCHAR(50)                  │   │  │
│  │ │ NomeCliente                  │ VARCHAR(200)                 │   │  │
│  │ │ Tipo                         │ VARCHAR(20) [PIX|CART...]   │   │  │
│  │ │ Severidade                   │ VARCHAR(20) [BAIXA|MÉDIA...] │   │  │
│  │ │ Status                       │ VARCHAR(20) [ABERTO|EM...]  │   │  │
│  │ │ DataCriacao (INDEX)          │ DateTime (UTC)              │   │  │
│  │ │ Descricao                    │ VARCHAR(1000) NULL          │   │  │
│  │ │ --- ÍNDICES ---              │                              │   │  │
│  │ │ • PK: Id                     │                              │   │  │
│  │ │ • UX: Protocolo              │                              │   │  │
│  │ │ • IX: Status, Severidade     │                              │   │  │
│  │ │ • IX: DataCriacao            │                              │   │  │
│  │ └──────────────────────────────────────────────────────────────┘   │  │
│  │                                                                      │  │
│  │ [Seed Data: 5 tickets pré-carregados]                              │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

## Fluxo de Dados - Criar Ticket Crítico

```
┌────────────────────────────────┐
│  GraphQL Mutation Request       │
│  criarTicket(                   │
│    protocolo: "PIX-2024-999"   │
│    nomeCliente: "João Silva"   │
│    tipo: PIX                   │
│    severidade: CRITICA         │
│  )                             │
└───────────┬────────────────────┘
            │
            ▼
┌────────────────────────────────────────────┐
│  Hot Chocolate - Mutation Resolver         │
│  Mutation.CriarTicket()                   │
│  • Valida entrada (CriarTicketInput)      │
│  • Cria objeto Ticket                     │
│  • DataCriacao = DateTime.UtcNow          │
│  • Status = ABERTO (padrão)               │
└────────────┬─────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────┐
│  Entity Framework Core (DbContext)         │
│  • context.Tickets.Add(ticket)            │
│  • context.SaveChangesAsync()             │
└────────────┬─────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────┐
│  SQL Server - INSERT Ticket               │
│  INSERT INTO Tickets VALUES (...)          │
│  → Retorna ID gerado                       │
└────────────┬─────────────────────────────┘
             │
             ▼
         SUCESSO!
         │
         └────┬──────────────────────────┐
              │                          │
              ▼                          ▼
      ┌─────────────────┐        ┌──────────────────────┐
      │ Severidade ==   │        │ Publicar Evento:     │
      │ CRITICA?        │        │ ITopicEventSender    │
      └────┬────────────┘        │ .SendAsync(          │
           │                     │   "TicketCriticoCr..." │
     ┌─────┴─────┐               │ )                    │
     │ SIM │NÃO  │               └──────────┬───────────┘
     ▼    ▼      │                          │
   PUBL  SKIP    │                          ▼
    │           │                  ┌──────────────────┐
    │           │                  │ In-Memory Topic  │
    │           │                  │ Pub/Sub Buffer   │
    │           │                  └────────┬─────────┘
    │           │                           │
    │           └───────────────┬───────────┘
    │                           │
    ▼                           ▼
┌──────────────────────────────────────────┐
│  Retornar TicketType para Cliente         │
│  {                                       │
│    id: 6,                                │
│    protocolo: "PIX-2024-999",           │
│    nomeCliente: "João Silva",           │
│    tipo: PIX,                           │
│    severidade: CRITICA,                 │
│    status: ABERTO,                      │
│    dataCriacao: "2024-05-30T10:30:45Z" │
│  }                                       │
└──────────────────────────────────────────┘
         │
         ▼
┌──────────────────────────────────────────────────┐
│  APENAS SE severidade == CRITICA:                │
│                                                  │
│  Subscribers conectados (via WebSocket)          │
│  recebem o evento em TEMPO REAL                  │
│  sem precisar fazer refresh!                     │
└──────────────────────────────────────────────────┘
```

---

## Estrutura de Pastas - Detalhada

```
TicketAPI/
│
├── 📁 Models/                          # Definição das entidades
│   ├── Enums.cs                        # Tipos enum (TipoReclamacao, NivelSeveridade, StatusTicket)
│   └── Ticket.cs                       # Entidade principal da aplicação
│
├── 📁 Data/                            # Acesso aos dados (Data Access Layer)
│   └── TicketDbContext.cs              # Entity Framework DbContext
│                                       # - Configuração das entidades
│                                       # - Mapeamento para SQL Server
│                                       # - Seed de dados iniciais
│
├── 📁 GraphQL/                         # Camada GraphQL
│   ├── 📁 Types/                       # Tipos GraphQL
│   │   ├── TicketType.cs               # Representação do Ticket em GraphQL
│   │   └── CriarTicketInput.cs         # Input para criar tickets
│   │
│   ├── 📁 Queries/                     # Operações de leitura
│   │   └── Query.cs                    # 10+ queries para buscar tickets
│   │
│   ├── 📁 Mutations/                   # Operações de escrita
│   │   └── Mutation.cs                 # 4 mutations (Criar, Atualizar, Deletar)
│   │
│   └── 📁 Subscriptions/               # Eventos em tempo real
│       └── Subscription.cs             # 2 subscriptions (Crítico, Qualquer)
│
├── 📁 Properties/
│   └── launchSettings.json             # Configuração de launch do Visual Studio
│
├── Program.cs                          # ⭐ Ponto de entrada e configuração
│                                       # - Registra Hot Chocolate
│                                       # - Configura Entity Framework
│                                       # - Aplica migrations
│                                       # - Executa seed
│
├── TicketAPI.csproj                    # Definição do projeto
│                                       # - Versão .NET 8.0
│                                       # - Referências de pacotes
│
├── appsettings.json                    # Configurações gerais
│                                       # - Connection String
│                                       # - Logging
│
├── appsettings.Development.json        # Configurações de desenvolvimento
│                                       # - Debug logging
│
├── .gitignore                          # Arquivo para ignorar no Git
│
├── README.md                           # 📖 Documentação COMPLETA
│                                       # - Setup
│                                       # - Exemplos
│                                       # - Explicações
│
├── QUICK_START.md                      # ⚡ Guia rápido (5 minutos)
│
└── EXEMPLOS_GRAPHQL.md                 # 📋 20+ Exemplos prontos
                                        # - Queries
                                        # - Mutations
                                        # - Subscriptions
```

---

## Tecnologias e Bibliotecas

| Tecnologia | Versão | Propósito |
|-----------|--------|----------|
| **ASP.NET Core** | 8.0 | Framework web |
| **Entity Framework Core** | 8.0 | ORM para banco de dados |
| **SQL Server** | 2019+ | Banco de dados relacional |
| **Hot Chocolate** | 14.0 | Servidor GraphQL |
| **Serilog** | 8.0 | Logging estruturado |

---

## Fluxo de Requisição GraphQL

```
Cliente
   │
   ├─ HTTP POST /graphql
   │  Headers: Content-Type: application/json
   │  Body: { query: "...", variables: {...} }
   │
   ▼
┌─────────────────────────────────────┐
│ Hot Chocolate Middleware            │
│ • Parse GraphQL Query               │
│ • Validação de Schema               │
│ • Autorização (se houver)           │
└──────────┬──────────────────────────┘
           │
           ▼
    ┌──────────────────┐
    │ Tipo da Operação?│
    └────┬──┬───┬──────┘
         │  │   │
    QUERY │  │ SUBSCRIPTION
         │  │   │
    Mutation
         │
    ┌────▼────────────────────────────┐
    │ Resolver (Query/Mutation)       │
    │ • Lógica de negócio             │
    │ • Acesso a dados (DbContext)    │
    │ • Validações                    │
    └────┬─────────────────────────────┘
         │
         ▼
    ┌────────────────────────────┐
    │ Entity Framework Core      │
    │ • LINQ to SQL              │
    │ • SQL Generation           │
    │ • Object Mapping           │
    └────┬───────────────────────┘
         │
         ▼
    ┌────────────────────────────┐
    │ SQL Server                 │
    │ • Executa Query SQL        │
    │ • Retorna Dados            │
    └────┬───────────────────────┘
         │
         ▼
    ┌────────────────────────────┐
    │ Mapear Resultados          │
    │ • Model → GraphQL Type     │
    │ • JSON Serialization       │
    └────┬───────────────────────┘
         │
         ▼
    ┌────────────────────────────┐
    │ HTTP 200 OK                │
    │ { "data": {...} }          │
    │ ou                         │
    │ { "errors": [...] }        │
    └────────────────────────────┘
```

---

## Ciclo de Vida da Subscription

```
1️⃣  Cliente abre Playground
2️⃣  Cliente cola subscription: ticketCriticoCriado
3️⃣  Cliente clica "Start"
4️⃣  Conexão WebSocket é estabelecida
5️⃣  Hot Chocolate registra o subscriber
6️⃣  Status: "Waiting for server events..."

    ... Aguardando ...

7️⃣  Outro usuário executa mutation: criarTicket(severidade: CRITICA)
8️⃣  Servidor valida e salva no BD (OK)
9️⃣  Servidor publica evento: ITopicEventSender.SendAsync("TicketCriticoCriado", ticket)
🔟  In-Memory Pub/Sub Buffer recebe o evento
1️⃣1️⃣ Hot Chocolate notifica TODOS os subscribers
1️⃣2️⃣ Cliente 1 recebe notificação em tempo real! ✓
    {
      "data": {
        "ticketCriticoCriado": {
          "protocolo": "PIX-2024-999",
          "nomeCliente": "João Silva",
          ...
        }
      }
    }

1️⃣3️⃣ Conexão WebSocket permanece aberta
1️⃣4️⃣ Cliente recebe próximos eventos assim que forem publicados
1️⃣5️⃣ Quando cliente fecha a aba, WebSocket é desconectado
```

---

## Performance e Índices

```
Índices criados no SQL Server para otimização:

PRIMARY KEY:
  • Tickets.Id

UNIQUE INDEX:
  • Tickets.Protocolo (busca rápida por protocolo)

COMPOSITE INDEX:
  • Tickets(Status, Severidade) 
    → Otimiza: filtros por status e severidade

SIMPLE INDEX:
  • Tickets.DataCriacao
    → Otimiza: ordenação e filtros por data

Resultado:
  Query de filtro: O(log n) em vez de O(n)
  Busca por protocolo: O(1) em vez de O(n)
```

---

## Diagrama de Segurança e CORS

```
┌────────────────────────────────┐
│  Navegador do Cliente          │
│  localhost:3000 (Frontend)     │
└────────────┬───────────────────┘
             │
             │ Origin: localhost:3000
             │ Preflight: OPTIONS request
             │
             ▼
┌──────────────────────────────────────┐
│  ASP.NET Core CORS Middleware        │
│                                      │
│  Policy: "AllowAll"                  │
│  • AllowAnyOrigin()                  │
│  • AllowAnyMethod()                  │
│  • AllowAnyHeader()                  │
└────────┬─────────────────────────────┘
         │
         ├─ Verifica permissões
         │
         ├─ Adiciona headers:
         │  • Access-Control-Allow-Origin: *
         │  • Access-Control-Allow-Methods: *
         │  • Access-Control-Allow-Headers: *
         │
         ▼
┌──────────────────────────────────────┐
│  Request é permitido!                │
│                                      │
│  GraphQL Banana Cake Pop pode:       │
│  ✓ Fazer requisições POST            │
│  ✓ Abrir WebSocket                   │
│  ✓ Enviar Headers customizados       │
└──────────────────────────────────────┘

⚠️ NOTA: Configuração aberta é para DESENVOLVIMENTO
   Para PRODUÇÃO, use origins específicos!
```

---

**Desenvolvido com ❤️ usando ASP.NET Core 8, Hot Chocolate e Entity Framework Core**
