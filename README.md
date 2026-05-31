# API GraphQL de Monitoramento de Tickets Bancários

Uma API GraphQL completa construída com ASP.NET Core 8, Hot Chocolate e Entity Framework Core para monitoramento de reclamações críticas de clientes bancários.

## 📋 Índice

- [Estrutura do Projeto](#estrutura-do-projeto)
- [Requisitos](#requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Exemplos de Uso](#exemplos-de-uso)
- [Entidades e Tipos](#entidades-e-tipos)
- [Operações GraphQL](#operações-graphql)
- [Subscriptions em Tempo Real](#subscriptions-em-tempo-real)
- [DataLoader](#dataloader)
- [Diagrama de Fluxo](#diagrama-de-fluxo)
- [Documentação Completa](#documentação-completa)

---

## 📁 Estrutura do Projeto

```
TicketAPI/
├── Models/
│   ├── Enums.cs                 # Definições dos enums (Tipo, Severidade, Status)
│   ├── Ticket.cs                # Modelo da entidade Ticket
│   └── Departamento.cs          # Modelo da entidade Departamento
├── Data/
│   └── TicketDbContext.cs       # Context do EF Core, configurações e seed
├── GraphQL/
│   ├── Queries/
│   │   └── Query.cs             # Queries GraphQL para leitura
│   ├── Mutations/
│   │   └── Mutation.cs          # Mutations GraphQL para modificação
│   ├── Subscriptions/
│   │   └── Subscription.cs      # Subscriptions GraphQL para eventos em tempo real
│   ├── DataLoaders/
│   │   └── DepartamentoDataLoader.cs  # DataLoader para evitar problema N+1
│   └── Types/
│       ├── TicketType.cs              # Tipo GraphQL para Ticket
│       ├── DepartamentoType.cs        # Tipo GraphQL para Departamento
│       ├── CriarTicketInput.cs        # Input type para criar ticket
│       └── CriarTicketInputComDeptInput.cs  # Input type com departamento
├── Migrations/
│   ├── 20260530212148_Initial.cs          # Migração inicial
│   └── 20260530220008_AddDepartamento.cs  # Migração para adicionar departamentos
├── Docs/                        # Documentação completa do projeto
│   ├── README.md                # Guia geral
│   ├── QUICK_START.md           # Início rápido
│   ├── ARQUITETURA.md           # Arquitetura detalhada
│   ├── EXEMPLOS_GRAPHQL.md      # Exemplos práticos de GraphQL
│   └── ...
├── Program.cs                    # Configuração e inicialização da aplicação
├── appsettings.json             # Configurações gerais
├── appsettings.Development.json # Configurações de desenvolvimento
├── TicketAPI.csproj             # Definição do projeto e dependências
├── Insomnia_GraphQL.yaml        # Coleção de requisições para Insomnia
└── README.md                     # Este arquivo

```

---

## 🔧 Requisitos

- **.NET 8.0 SDK** ou posterior
- **SQL Server** 2019 ou posterior (ou SQL Server LocalDB para desenvolvimento)
- **Visual Studio 2022** ou **Visual Studio Code** com extensões C#

### Dependências Principais

- `Microsoft.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.0)
- `HotChocolate.AspNetCore` (14.0.0)
- `HotChocolate.Subscriptions.InMemory` (14.0.0)

---

## 🚀 Instalação e Configuração

### 1. Clonar ou Criar o Projeto

```bash
# Se usando Git
git clone <repository-url>
cd TicketAPI

# Ou navegar até o diretório do projeto
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Configurar o Banco de Dados

#### Opção A: Usando SQL Server LocalDB (Recomendado para Desenvolvimento)

A string de conexão padrão em `appsettings.json` já utiliza LocalDB:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketAPI;Trusted_Connection=true;"
```

#### Opção B: Usando SQL Server Express ou Instância Remota

Edite `appsettings.json`:

```json
"DefaultConnection": "Server=seu-servidor;Database=TicketAPI;User Id=sa;Password=sua-senha;"
```

### 4. Aplicar Migrations (Criar Banco de Dados)

```bash
# Restaura todas as migrations pendentes
dotnet ef database update

# Ou se estiver usando Package Manager Console no Visual Studio
Update-Database
```

### 5. Executar a Aplicação

```bash
dotnet run

# Ou no Visual Studio, pressione F5
```

A aplicação será iniciada em `https://localhost:5001` (HTTPS) ou `http://localhost:5000` (HTTP).

---

## 📖 Exemplos de Uso

### Acessar o GraphQL Playground

1. Abra o navegador e acesse: `http://localhost:5000/graphql`
2. O **GraphQL Banana Cake Pop** será carregado automaticamente
3. Você pode executar queries, mutations e subscriptions

---

## 🏷️ Entidades e Tipos

### Enum: TipoReclamacao

```graphql
enum TipoReclamacao {
  PIX           # Reclamações de transferências PIX
  CARTAO        # Reclamações de cartão de crédito/débito
  CONTA         # Problemas na conta bancária
  FRAUDE        # Denúncia de fraude
  EMPRESTIMO    # Questões sobre empréstimos
}
```

### Enum: NivelSeveridade

```graphql
enum NivelSeveridade {
  BAIXA         # Impacto baixo
  MEDIA         # Impacto médio
  ALTA          # Impacto alto
  CRITICA       # Impacto crítico - Requer atenção imediata
}
```

### Enum: StatusTicket

```graphql
enum StatusTicket {
  ABERTO        # Ticket recém-criado
  EMANALISE     # Sendo analisado pela equipe
  RESOLVIDO     # Problema resolvido
}
```

### Type: Ticket

```graphql
type Ticket {
  id: Int!                  # ID único no banco
  protocolo: String!        # Número do protocolo
  nomeCliente: String!      # Nome do cliente
  tipo: TipoReclamacao!     # Tipo de reclamação
  severidade: NivelSeveridade!  # Nível de severidade
  status: StatusTicket!     # Status atual
  dataCriacao: DateTime!    # Data de criação (UTC)
  descricao: String         # Descrição opcional
}
```

### Input: CriarTicketInput

```graphql
input CriarTicketInput {
  protocolo: String!           # Obrigatório
  nomeCliente: String!         # Obrigatório
  tipo: TipoReclamacao!        # Obrigatório
  severidade: NivelSeveridade! # Obrigatório
  descricao: String            # Opcional
}
```

---

## 🔍 Operações GraphQL

### QUERIES - Leitura de Dados

#### 1️⃣ Buscar Todos os Tickets

```graphql
query {
  tickets {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
  }
}
```

#### 2️⃣ Buscar Ticket por ID

```graphql
query {
  ticketPorId(id: 1) {
    id
    protocolo
    nomeCliente
    descricao
  }
}
```

#### 3️⃣ Buscar Ticket por Protocolo

```graphql
query {
  ticketPorProtocolo(protocolo: "PIX-2024-001") {
    id
    protocolo
    nomeCliente
    tipo
  }
}
```

#### 4️⃣ Filtrar por Status

```graphql
query {
  ticketsPorStatus(status: ABERTO) {
    id
    protocolo
    nomeCliente
    status
  }
}
```

#### 5️⃣ Filtrar por Severidade

```graphql
query {
  ticketsPorSeveridade(severidade: CRITICA) {
    id
    protocolo
    nomeCliente
    severidade
    dataCriacao
  }
}
```

#### 6️⃣ Filtrar por Tipo

```graphql
query {
  ticketsPorTipo(tipo: PIX) {
    id
    protocolo
    nomeCliente
    tipo
  }
}
```

#### 7️⃣ Buscar Tickets Críticos Abertos (Conveniência)

```graphql
query {
  ticketsCriticosAbertos {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    dataCriacao
  }
}
```

#### 8️⃣ Contar Total de Tickets

```graphql
query {
  totalTickets
}
```

#### 9️⃣ Contar Tickets por Status

```graphql
query {
  totalTicketsPorStatus(status: CRITICA)
}
```

---

### ✏️ MUTATIONS - Modificação de Dados

#### 1️⃣ Criar Novo Ticket

**⚠️ IMPORTANTE:** Se `severidade = CRITICA`, um evento é publicado para a subscription!

```graphql
mutation {
  criarTicket(input: {
    protocolo: "PIX-2024-999"
    nomeCliente: "João Silva"
    tipo: PIX
    severidade: CRITICA
    descricao: "Transferência PIX de R$ 5.000 não recebida"
  }) {
    id
    protocolo
    nomeCliente
    status
    dataCriacao
  }
}
```

**Resposta esperada:**

```json
{
  "data": {
    "criarTicket": {
      "id": 6,
      "protocolo": "PIX-2024-999",
      "nomeCliente": "João Silva",
      "status": "ABERTO",
      "dataCriacao": "2024-05-30T10:30:45Z"
    }
  }
}
```

#### 2️⃣ Atualizar Status do Ticket

```graphql
mutation {
  atualizarStatusTicket(id: 1, novoStatus: EMANALISE) {
    id
    protocolo
    status
  }
}
```

#### 3️⃣ Atualizar Severidade do Ticket

```graphql
mutation {
  atualizarSeveridadeTicket(id: 1, novaSeveridade: ALTA) {
    id
    protocolo
    severidade
  }
}
```

#### 4️⃣ Deletar Ticket

```graphql
mutation {
  deletarTicket(id: 1)
}
```

---

## 🔔 Subscriptions em Tempo Real

### Como Funcionam as Subscriptions

As subscriptions permitem que clientes recebam notificações **em tempo real** via WebSocket quando eventos ocorrem no servidor.

**Fluxo:**

```
┌─────────────────────────────────────────────────────────────┐
│ Cliente A conecta e subscrevê "ticketCriticoCriado"         │
│ (conexão WebSocket é aberta)                                 │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Cliente B executa mutation "criarTicket" com severidade     │
│ CRITICA                                                      │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Servidor publica evento em "TicketCriticoCriado"            │
│ (usando ITopicEventSender)                                   │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Cliente A recebe o evento em tempo real                      │
│ Pode tomar ações imediatas (alertas, notificações, etc)    │
└─────────────────────────────────────────────────────────────┘
```

### 1️⃣ Subscription - Ticket Crítico Criado

**Notifica quando um ticket crítico é criado**

```graphql
subscription {
  ticketCriticoCriado {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
  }
}
```

### 2️⃣ Subscription - Qualquer Ticket Criado

**Notifica sobre QUALQUER ticket criado (não apenas críticos)**

```graphql
subscription {
  ticketCriado {
    protocolo
    nomeCliente
    tipo
    severidade
    dataCriacao
  }
}
```

### Como Testar Subscriptions no GraphQL Banana Cake Pop

1. **Abra duas abas** no Banana Cake Pop (`http://localhost:5000/graphql`)

2. **Na primeira aba (Subscription):**
   - Cole a query de subscription
   - Clique em "Start"
   - Verá: `Waiting for server events...`

3. **Na segunda aba (Mutation):**
   - Execute a mutation `criarTicket` com `severidade: CRITICA`

4. **Volte para a primeira aba:**
   - Você verá a notificação em tempo real com os dados do novo ticket!

---

## 📊 Diagrama de Fluxo

### Fluxo de Criação de Ticket Crítico

```
┌─────────────────────┐
│  Cliente GraphQL    │
│  (Banana Cake Pop)  │
└──────────┬──────────┘
           │
           │ POST /graphql
           │ mutation { criarTicket(...) }
           ↓
┌──────────────────────────┐
│  ASP.NET Core Server     │
│  Hot Chocolate           │
└──────────┬───────────────┘
           │
           ├─────────────────────────────────────┐
           │                                     │
           ↓                                     ↓
    ┌──────────────┐              ┌───────────────────────┐
    │ Validar Input│              │ Salvar no BD            │
    └──────────────┘              │ Entity Framework Core  │
           │                       └───────────────────────┘
           │                               │
           └───────────────┬───────────────┘
                           ↓
                   ┌───────────────┐
                   │ Severidade    │
                   │ == CRITICA?   │
                   └───────┬───────┘
                           │
                    ┌──────┴──────┐
                    │ SIM   │NÃO  │
                    ↓      ↓
              ┌────────┐ ┌─────────┐
              │ Publicar│ │ Retornar│
              │ Evento  │ │ Ticket  │
              └────┬───┘ └─────────┘
                   │
                   ↓
         ┌──────────────────────┐
         │ ITopicEventSender    │
         │ SendAsync(...)       │
         └──────────┬───────────┘
                    │
                    ↓
         ┌────────────────────────┐
         │ Subscriptions (WebSocket)
         │ - ticketCriticoCriado  │
         │ - ticketCriado         │
         └────────────────────────┘
                    │
                    ↓
         ┌────────────────────────┐
         │ Clientes Conectados    │
         │ Recebem Evento em      │
         │ Tempo Real ✓           │
         └────────────────────────┘
```

---

## 🛠️ Explicação Detalhada do Código

### 1. Modelos (Models)

**`Models/Enums.cs`**: Define os três enumeradores usados na aplicação
- `TipoReclamacao`: Categoria da reclamação
- `NivelSeveridade`: Importância/urgência
- `StatusTicket`: Fase atual do ticket

**`Models/Ticket.cs`**: Entidade principal que representa um ticket bancário
- Propriedades de identificação e descrição
- Relacionamento com Departamento (FK)
- Timestamps automáticos

**`Models/Departamento.cs`**: Entidade que representa departamentos
- ID, Nome, Email, Telefone
- Relacionamento com múltiplos Tickets

### 2. Data Access (Data)

**`Data/TicketDbContext.cs`**: 
- Gerencia a conexão com SQL Server
- Define o mapeamento de propriedades para colunas do BD
- Configura índices para melhor performance
- Implementa o seed inicial de dados

### 3. GraphQL Server

**`GraphQL/Queries/Query.cs`**: Define as operações de leitura
- `GetTickets()`: Lista todos
- `GetTicketPorId()`: Por ID
- `GetTicketsPorStatus()`: Filtra por status
- `GetTicketsPorSeveridade()`: Filtra por severidade
- etc.

**`GraphQL/Mutations/Mutation.cs`**: Define as operações de escrita
- `CriarTicket()`: Cria novo ticket
  - Valida entrada
  - Salva no BD
  - **Se CRITICA**: Publica evento
- `AtualizarStatusTicket()`: Altera status
- `AtualizarSeveridadeTicket()`: Altera severidade
- `DeletarTicket()`: Remove ticket

**`GraphQL/Subscriptions/Subscription.cs`**: Define eventos em tempo real
- `TicketCriticoCriado`: Notifica sobre críticos
- `TicketCriado`: Notifica sobre qualquer novo

### 4. Tipos GraphQL

**`GraphQL/Types/TicketType.cs`**: Representa o Ticket para GraphQL
- Field resolver `Departamento()` com DataLoader
- Converte modelo Ticket para tipo GraphQL

**`GraphQL/Types/DepartamentoType.cs`**: Representa o Departamento para GraphQL
- Propriedades de identificação e contato
- Método factory `FromDepartamento()`

**`GraphQL/Types/CriarTicketInput.cs`**: Dados de entrada para mutation
- Input type básico para criar ticket

**`GraphQL/Types/CriarTicketInputComDeptInput.cs`**: Dados de entrada com departamento
- Input type estendido incluindo `departamentoId`

### 5. DataLoaders

**`GraphQL/DataLoaders/DepartamentoDataLoader.cs`**: 
- Implementa batch loading para departamentos
- Evita problema N+1 queries
- Agrupa múltiplas requisições em uma única query
- Registrado via `AddDataLoader<DepartamentoDataLoader>()` em Program.cs

### 6. Configuração Principal

**`Program.cs`**: 
- Configura Entity Framework Core com SQL Server
- Registra Hot Chocolate GraphQL com tipos explícitos
- Registra DataLoader para departamentos
- Configura subscriptions em memória
- Aplica migrations e seed automático
- Define endpoints de GraphQL

---

## � DataLoader

O projeto implementa **DataLoader** para evitar o problema **N+1 queries**:

### O que é o Problema N+1?

Sem DataLoader:
- Query de 100 tickets: 1 query
- Carregar departamento para cada ticket: 100 queries
- **Total: 101 queries** ❌

Com DataLoader:
- Query de 100 tickets: 1 query
- Carregar TODOS os departamentos em 1 única query: 1 query
- **Total: 2 queries** ✅

### Como Funciona

```csharp
// Em GraphQL/DataLoaders/DepartamentoDataLoader.cs
public class DepartamentoDataLoader : BatchDataLoader<int, Departamento>
{
    protected override async Task<IReadOnlyDictionary<int, Departamento>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        // Carrega TODOS os departamentos com os IDs especificados em UMA ÚNICA query
        var departamentos = await context.Departamentos
            .Where(d => keys.Contains(d.Id))
            .ToListAsync(cancellationToken);
        
        return departamentos.ToDictionary(d => d.Id);
    }
}
```

### Uso no TicketType

```csharp
// Em GraphQL/Types/TicketType.cs
public async Task<DepartamentoType?> Departamento(
    DepartamentoDataLoader dataLoader,
    CancellationToken cancellationToken)
{
    if (DepartamentoId == 0) return null;
    
    var departamento = await dataLoader.LoadAsync(DepartamentoId, cancellationToken);
    return departamento != null ? DepartamentoType.FromDepartamento(departamento) : null;
}
```

### Registro em Program.cs

```csharp
builder.Services.AddGraphQLServer()
    .AddDataLoader<DepartamentoDataLoader>()
    // ... outros registros
```

---

## 🔐 Boas Práticas Implementadas

1. ✅ **Injeção de Dependências**: DbContext e serviços injetados
2. ✅ **Async/Await**: Operações assíncronas em toda aplicação
3. ✅ **Validação**: Checks de nulidade e existência
4. ✅ **Documentação**: Comentários XML em cada método
5. ✅ **Estrutura Modular**: Separação clara de responsabilidades
6. ✅ **Entity Framework Fluent API**: Configuração limpa do modelo
7. ✅ **CORS Habilitado**: Permite conexões do Playground
8. ✅ **Índices**: Melhor performance nas queries
9. ✅ **Timestamps**: Registro automático de data/hora
10. ✅ **Real-time**: Subscriptions com WebSocket
11. ✅ **DataLoader**: Evita problema N+1 em relacionamentos
12. ✅ **Tratamento de Erros**: GraphQL errors com códigos específicos

---

## 🐛 Troubleshooting

### Erro: "Connection to SQL Server failed"

**Solução:**
- Verifique se SQL Server LocalDB está instalado
- Execute: `sqllocaldb info mssqllocaldb`
- Se não existir, crie: `sqllocaldb create mssqllocaldb`

### Erro: "Table 'TicketAPI.dbo.Tickets' doesn't exist"

**Solução:**
- Aplique as migrations: `dotnet ef database update`
- Ou no Package Manager: `Update-Database`

### Subscription não funciona

**Solução:**
- Verifique se a conexão WebSocket está aberta
- Teste com duas abas do Banana Cake Pop simultaneamente
- Verifique o console da aplicação para erros

### Porta 5000 já em uso

**Solução:**
- Use outra porta: `dotnet run --urls http://localhost:5001`
- Ou encerre o processo usando a porta

---

## 📚 Documentação Completa

### Documentação do Projeto (Pasta `/Docs`)

- **[START_HERE.md](./Docs/START_HERE.md)** - ⭐ Comece por aqui!
- **[QUICK_START.md](./Docs/QUICK_START.md)** - Início rápido em 5 minutos
- **[README.md](./Docs/README.md)** - Documentação geral
- **[ARQUITETURA.md](./Docs/ARQUITETURA.md)** - Arquitetura detalhada do projeto
- **[EXEMPLOS_GRAPHQL.md](./Docs/EXEMPLOS_GRAPHQL.md)** - Exemplos práticos de GraphQL
- **[FLUXO_SUBSCRIPTION.md](./Docs/FLUXO_SUBSCRIPTION.md)** - Como funcionam as subscriptions
- **[DOCKER.md](./Docs/DOCKER.md)** - Instruções para rodar com Docker
- **[CHECKLIST.md](./Docs/CHECKLIST.md)** - Checklist de desenvolvimento
- **[ARQUIVO_LISTA.md](./Docs/ARQUIVO_LISTA.md)** - Lista completa de arquivos

### Recursos Externos

- [Hot Chocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [GraphQL Official Site](https://graphql.org/)

---

## 📝 Licença

Este projeto é fornecido como exemplo educacional.

---

**Desenvolvido com ❤️ usando ASP.NET Core 8, Hot Chocolate e Entity Framework Core**
