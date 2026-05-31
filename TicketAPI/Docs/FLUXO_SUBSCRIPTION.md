# 🔔 Fluxo Detalhado de Subscriptions em Tempo Real

## Visão Geral

As **subscriptions** no GraphQL permitem que clientes recebam **notificações em tempo real** quando eventos ocorrem no servidor, sem precisar fazer polling (requisições repetidas).

---

## Como Funcionam as Subscriptions

### 📡 Protocolo de Comunicação

```
Cliente              Servidor
  │                    │
  ├─ WebSocket ────────┤ Abre conexão
  │                    │
  ├─ Subscription ─────┤ Registra interesse em eventos
  │                    │
  │      ...           │ Aguardando eventos
  │      ...           │
  │                    ├─ Evento disparado! 🔔
  │                    │ (Mutation que cria ticket crítico)
  │                    │
  ├─ Notificação ──────┤ Envia dados do evento
  │                    │
  │      ...           │ Continua aguardando próximos
  │      ...           │
  ├─ Desconecta ───────┤ Fecha WebSocket
  │                    │
```

---

## Fluxo Completo: Criar Ticket Crítico com Subscription

### 👥 Atores

- **Cliente 1** (Monitor) - Conectado à subscription
- **Cliente 2** (Usuário) - Cria um novo ticket
- **Servidor** (ASP.NET Core + Hot Chocolate)
- **Banco de Dados** (SQL Server)
- **Sistema de Pub/Sub** (In-Memory)

### ⏱️ Sequência de Eventos

```
TEMPO  |  ATOR      |  AÇÃO
───────┼────────────┼──────────────────────────────────────────

T1     │ Cliente 1  │ ✅ Abre GraphQL Banana Cake Pop
       │            │    Navegador: http://localhost:5000/graphql
       │            │    Estabelece conexão WebSocket

T2     │ Cliente 1  │ ✅ Escreve subscription:
       │            │    subscription {
       │            │      ticketCriticoCriado {
       │            │        protocolo
       │            │        nomeCliente
       │            │        severidade
       │            │      }
       │            │    }

T3     │ Servidor   │ ✅ Recebe subscription
       │            │    • Valida sintaxe GraphQL
       │            │    • Registra Cliente 1 no tópico
       │            │      "TicketCriticoCriado"

T4     │ Cliente 1  │ ✅ Clica "Start"
       │            │    • WebSocket conecta
       │            │    • Status: "Waiting for server events..."

T5     │ Cliente 2  │ ✅ Abre outra aba do Playground

T6     │ Cliente 2  │ ✅ Escreve mutation:
       │            │    mutation {
       │            │      criarTicket(input: {
       │            │        protocolo: "PIX-2024-999"
       │            │        nomeCliente: "João Silva"
       │            │        tipo: PIX
       │            │        severidade: CRITICA  ⚠️
       │            │      }) {
       │            │        id
       │            │        protocolo
       │            │      }
       │            │    }

T7     │ Servidor   │ ✅ Recebe mutation
       │            │    • Parsing da requisição HTTP POST
       │            │    • Routing para Mutation.CriarTicket()

T8     │ Servidor   │ ✅ Executa mutation:
       │  (Mutation)│    • Valida CriarTicketInput
       │            │    • Cria objeto Ticket
       │            │    • DataCriacao = DateTime.UtcNow
       │            │    • Status = ABERTO (padrão)

T9     │ Servidor   │ ✅ Persiste no banco:
       │  (EF Core) │    • context.Tickets.Add(ticket)
       │            │    • context.SaveChangesAsync()
       │            │    ↓
       │ Banco      │    INSERT INTO Tickets VALUES (...)
       │            │    ✓ Retorna Id = 6

T10    │ Servidor   │ ✅ Verifica severidade:
       │            │    if (input.Severidade == CRITICA) {
       │            │      // PUBLICA EVENTO!
       │            │    }

T11    │ Servidor   │ ✅ Publica evento:
       │  (ITopicEvent│  await eventSender.SendAsync(
       │   Sender)  │    "TicketCriticoCriado",  ← Tópico
       │            │    ticket                  ← Dados
       │            │  );

T12    │ Servidor   │ ✅ In-Memory Pub/Sub:
       │  (Buffer)  │    • Evento entra na fila
       │            │    • Procura subscribers do tópico
       │            │    │   "TicketCriticoCriado"
       │            │    └─→ Encontra Cliente 1!

T13    │ Servidor   │ ✅ Serializa dados para JSON:
       │            │    {
       │            │      "data": {
       │            │        "ticketCriticoCriado": {
       │            │          "protocolo": "PIX-2024-999",
       │            │          "nomeCliente": "João Silva",
       │            │          "severidade": "CRITICA"
       │            │        }
       │            │      }
       │            │    }

T14    │ Servidor   │ ✅ Envia via WebSocket para Cliente 1:
       │            │    (mensagem em tempo real!)

T15    │ Cliente 1  │ 🔔 RECEBE NOTIFICAÇÃO!
       │            │    ✅ GraphQL Playground exibe dados:
       │            │    {
       │            │      "data": {
       │            │        "ticketCriticoCriado": {
       │            │          "protocolo": "PIX-2024-999",
       │            │          "nomeCliente": "João Silva",
       │            │          "severidade": "CRITICA"
       │            │        }
       │            │      }
       │            │    }
       │            │
       │            │    ⚠️ SEM REFRESH! ⚠️
       │            │    ⏱️  Latência: ~100-200ms

T16    │ Cliente 2  │ ✅ Recebe resposta da mutation:
       │            │    {
       │            │      "data": {
       │            │        "criarTicket": {
       │            │          "id": 6,
       │            │          "protocolo": "PIX-2024-999"
       │            │        }
       │            │      }
       │            │    }

T17    │ Cliente 1  │ ✅ Continua conectado à subscription
       │            │    Status: "Waiting for server events..."
       │            │    Pronto para próxima notificação

```

---

## Diagrama de Estado

```
┌─────────────────────────────────────────────────┐
│         CLIENTE 1: SUBSCRIPTION LIFECYCLE       │
└─────────────────────────────────────────────────┘

          ┌─────────────────────────┐
          │   ESTADO: DESCONECTADO  │
          └────────────┬────────────┘
                       │ Abre aba do Playground
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: CONECTADO     │
          │   (HTTP/HTTPS)          │
          └────────────┬────────────┘
                       │ Escreve subscription
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: ENVIANDO      │
          │   (GraphQL Query)       │
          └────────────┬────────────┘
                       │ Hot Chocolate processa
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: AGUARDANDO    │
          │   (WebSocket Open)      │
          │                         │
          │ Waiting for events...   │
          └────────────┬────────────┘
                       │ Evento ocorre!
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: RECEBENDO     │
          │   (Data Transfer)       │
          │                         │
          │ 🔔 Notificação!         │
          └────────────┬────────────┘
                       │ Processa e exibe
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: AGUARDANDO    │
          │   (Back to listening)   │
          └────────────┬────────────┘
                       │ ... (volta para aguardando)
                       │ (pode receber múltiplos eventos)
                       │
                       ▼ Usuário clica X
          ┌─────────────────────────┐
          │   ESTADO: FECHANDO      │
          │   (WebSocket Close)     │
          └────────────┬────────────┘
                       │ Conexão encerrada
                       ▼
          ┌─────────────────────────┐
          │   ESTADO: DESCONECTADO  │
          │   (Remover do buffer)   │
          └─────────────────────────┘
```

---

## Múltiplos Clientes Simultâneos

```
┌───────────────────────────────────────────────────────────────┐
│                      SERVIDOR                                  │
├───────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌─────────────────────────────────────┐                     │
│  │  HOT CHOCOLATE - PUB/SUB BUFFER     │                     │
│  │                                     │                     │
│  │  Tópico: "TicketCriticoCriado"     │                     │
│  │  Subscribers:                       │                     │
│  │  • Cliente 1 ───────────┐          │                     │
│  │  • Cliente 2 ───────────┤───────┐  │                     │
│  │  • Cliente 3 ───────────┤───────┤─┐│                     │
│  │                          │       │ │                     │
│  └──────────────────────────┼───────┼─┼─────────────────────┘
│                             │       │ │
│                      WebSocket Connections
│                             │       │ │
│                             ▼       ▼ ▼
│           ┌──────────┬──────────┬──────────┐
│           │ Cliente1 │ Cliente2 │ Cliente3 │
│           │ Notif. ✓ │ Notif. ✓ │ Notif. ✓ │
│           └──────────┴──────────┴──────────┘
│
│ Latência ~100-200ms para TODOS simultaneamente!
└───────────────────────────────────────────────────────────────┘
```

---

## Implementação no Hot Chocolate

### Subscription Type (Servidor)

```csharp
public class Subscription
{
    [Subscribe]
    public async IAsyncEnumerable<TicketType> TicketCriticoCriado(
        [EventMessage] Ticket ticket)
    {
        // Cada vez que um evento "TicketCriticoCriado" é publicado,
        // este método é chamado e o ticket é enviado para o cliente
        yield return TicketType.FromTicket(ticket);
    }
}
```

**O que acontece:**
1. `[Subscribe]` marca como subscription
2. `[EventMessage]` recebe o evento publicado
3. `yield return` envia para o cliente
4. `IAsyncEnumerable` permite múltiplos eventos

### Mutation que Dispara (Servidor)

```csharp
public async Task<TicketType> CriarTicket(
    CriarTicketInput input,
    TicketDbContext context,
    [Service] ITopicEventSender eventSender)
{
    // ... criar e salvar ticket ...

    if (input.Severidade == NivelSeveridade.CRITICA)
    {
        // ⭐ Publica evento para todos os subscribers
        await eventSender.SendAsync(
            "TicketCriticoCriado",  // Nome do tópico
            ticket                   // Dados do evento
        );
    }

    return TicketType.FromTicket(ticket);
}
```

**O que acontece:**
1. `ITopicEventSender` injeta o publisher
2. `SendAsync` publica evento
3. Todos os subscribers recebem simultaneamente

### Query no Cliente (Playground)

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

**O que acontece:**
1. Cliente envia subscription via WebSocket
2. Hot Chocolate registra cliente como subscriber
3. Aguarda eventos
4. Quando evento chega, dados são enviados para cliente

---

## Performance e Otimização

### 📊 Throughput

```
Cenário: 1000 clientes conectados à subscription

Evento Publicado (CriarTicket)
         ↓
Hot Chocolate Buffer
         ↓
Serializa para JSON
         ↓
Envia para 1000 WebSocket connections
         ↓
Tempo Total: ~500-1000ms (depende de rede)

Latência por cliente: ~100-200ms
```

### 💾 Memória

```
Por Subscriber: ~10-20KB
1000 Subscribers: ~10-20MB

Mais otimizado que polling:
- Sem requisições HTTP repetidas
- Sem parsing de queries
- Apenas dados necessários
```

### 🔄 Escalabilidade

**In-Memory (Atual):**
- ✅ Perfeito para desenvolvimento e pequenos ambientes
- ✅ Baixa latência (~100ms)
- ⚠️ Limitado a um servidor
- ⚠️ Subscriptions perdidas se servidor reinicia

**Melhorias Possíveis:**

```
┌─ Redis Pub/Sub
│  • Suporta múltiplos servidores
│  • Subscriptions persistem
│  • Escalável para 10k+ clientes
│
├─ RabbitMQ / Azure Service Bus
│  • Queue durável
│  • Retry mechanism
│  • Dead-letter queues
│
└─ Kafka
   • Altíssimo throughput
   • Replicação
   • Exactly-once semantics
```

---

## Teste Passo a Passo

### 📝 Passo 1: Preparar Cliente 1 (Subscriber)

1. Abra: http://localhost:5000/graphql
2. Aba 1 do navegador
3. Cole a subscription:

```graphql
subscription {
  ticketCriticoCriado {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    dataCriacao
  }
}
```

4. Clique "Play" ou "Start"
5. Status: "Waiting for server events..."

---

### 📝 Passo 2: Preparar Cliente 2 (Publisher)

1. Abra novo Playground: http://localhost:5000/graphql
2. Aba 2 do navegador
3. Cole a mutation:

```graphql
mutation {
  criarTicket(input: {
    protocolo: "TEST-REALTIME"
    nomeCliente: "teste real-time"
    tipo: PIX
    severidade: CRITICA
  }) {
    id
    protocolo
    status
  }
}
```

---

### 📝 Passo 3: Executar Mutation

1. Na Aba 2, clique "Play"
2. **Mutation é executada**
3. Ticket crítico é criado
4. **Evento é publicado** ⭐

---

### 📝 Passo 4: Ver Notificação em Tempo Real

1. **Volte para Aba 1** (Subscription)
2. **Você verá a notificação!** 🔔

```json
{
  "data": {
    "ticketCriticoCriado": {
      "id": 7,
      "protocolo": "TEST-REALTIME",
      "nomeCliente": "teste real-time",
      "tipo": "PIX",
      "severidade": "CRITICA",
      "dataCriacao": "2024-05-30T10:45:32.123Z"
    }
  }
}
```

---

## Casos de Uso Reais

### 🏦 Banco
- Monitor vê tickets críticos em tempo real
- Sem refresh / sem polling
- Resposta imediata a fraudes

### 📦 Ecommerce
- Admin vê novos pedidos urgentes
- Chat em tempo real (similar)
- Notificações de stock baixo

### 🏥 Saúde
- Monitor de pacientes críticos
- Alertas de saturação de leitos
- Notificações de exames urgentes

### 🎮 Gaming
- Jogadores recebem notificações de eventos
- Matchmaking updates
- Tournament announcements

---

## Troubleshooting

### ❌ "Subscription recebe, mas não vejo dados"

**Possível causa:** Mutation não está sendo executada
**Solução:** 
- Execute a mutation em outra aba
- Certifique-se de que `severidade = CRITICA`
- Verifique logs do servidor

### ❌ "Subscription mostra: 'Connection lost'"

**Possível causa:** WebSocket desconectou
**Solução:**
- Clique "Start" novamente
- Verifique connection de rede
- Veja console do navegador (F12)

### ❌ "Não recebo notificações"

**Possível causa:** Tópico incorreto ou evento não publicado
**Solução:**
```csharp
// Verifique em Program.cs:
.AddInMemorySubscriptions()  // ✓ Configurado?

// Verifique em Mutation.cs:
if (input.Severidade == NivelSeveridade.CRITICA)
{
    await eventSender.SendAsync("TicketCriticoCriado", ticket);  // ✓ Tema correto?
}
```

### ❌ "Multiple subscriptions funcionam, mas lentamente"

**Possível causa:** Muitos clients, buffer sobrecarregado
**Solução:**
- Implementar Redis para escalabilidade
- Otimizar serialização
- Adicionar compression

---

## Resumo

| Aspecto | Detalhes |
|--------|----------|
| **Protocolo** | WebSocket (HTTP/1.1 Upgrade) |
| **Tópicos** | In-Memory (string) |
| **Latência** | ~100-200ms |
| **Escalabilidade** | In-Memory limitado |
| **Confiabilidade** | Perdido se servidor reinicia |
| **Casos de Uso** | Real-time notifications, live updates |

---

**Desenvolvido com ❤️ usando Hot Chocolate GraphQL Server**
