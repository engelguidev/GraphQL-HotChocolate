# 📋 Exemplos de Queries, Mutations e Subscriptions GraphQL
# Copie e cole esses exemplos no GraphQL Banana Cake Pop
# Acesso: http://localhost:5000/graphql

# =====================================================
# 🔍 QUERIES - LEITURA DE DADOS
# =====================================================

# Query 1: Buscar TODOS os Tickets
# Retorna lista completa de todos os tickets ordenados por data descendente
query BuscarTodosTickets {
  tickets {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
    descricao
  }
}

# ─────────────────────────────────────────────────────

# Query 2: Buscar Ticket por ID específico
# Útil para buscar um ticket específico
query BuscarTicketPorId {
  ticketPorId(id: 1) {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
    descricao
  }
}

# ─────────────────────────────────────────────────────

# Query 3: Buscar Ticket por Protocolo
# Busca por número do protocolo
query BuscarTicketPorProtocolo {
  ticketPorProtocolo(protocolo: "PIX-2024-001") {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
  }
}

# ─────────────────────────────────────────────────────

# Query 4: Filtrar Tickets por Status = ABERTO
# Mostra apenas tickets ainda abertos
query TicketsAbertos {
  ticketsPorStatus(status: ABERTO) {
    id
    protocolo
    nomeCliente
    severidade
    dataCriacao
  }
}

# ─────────────────────────────────────────────────────

# Query 5: Filtrar Tickets por Status = EMANALISE
query TicketsEmAnalise {
  ticketsPorStatus(status: EMANALISE) {
    id
    protocolo
    nomeCliente
    severidade
  }
}

# ─────────────────────────────────────────────────────

# Query 6: Filtrar Tickets por Status = RESOLVIDO
query TicketsResolvidos {
  ticketsPorStatus(status: RESOLVIDO) {
    id
    protocolo
    nomeCliente
    tipo
  }
}

# ─────────────────────────────────────────────────────

# Query 7: Filtrar por Severidade = CRITICA
# ⚠️ Tickets que exigem atenção imediata
query TicketsCriticos {
  ticketsPorSeveridade(severidade: CRITICA) {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
  }
}

# ─────────────────────────────────────────────────────

# Query 8: Filtrar por Severidade = ALTA
query TicketsAltos {
  ticketsPorSeveridade(severidade: ALTA) {
    id
    protocolo
    nomeCliente
    severidade
  }
}

# ─────────────────────────────────────────────────────

# Query 9: Filtrar por Severidade = MEDIA
query TicketsMedias {
  ticketsPorSeveridade(severidade: MEDIA) {
    id
    protocolo
    nomeCliente
  }
}

# ─────────────────────────────────────────────────────

# Query 10: Filtrar por Severidade = BAIXA
query TicketsBaixos {
  ticketsPorSeveridade(severidade: BAIXA) {
    id
    protocolo
    nomeCliente
  }
}

# ─────────────────────────────────────────────────────

# Query 11: Filtrar por Tipo = PIX
query TicketsPix {
  ticketsPorTipo(tipo: PIX) {
    id
    protocolo
    nomeCliente
    tipo
    severidade
  }
}

# ─────────────────────────────────────────────────────

# Query 12: Filtrar por Tipo = CARTAO
query TicketsCartao {
  ticketsPorTipo(tipo: CARTAO) {
    id
    protocolo
    nomeCliente
    tipo
  }
}

# ─────────────────────────────────────────────────────

# Query 13: Filtrar por Tipo = FRAUDE
query TicketsFraude {
  ticketsPorTipo(tipo: FRAUDE) {
    id
    protocolo
    nomeCliente
    tipo
    severidade
  }
}

# ─────────────────────────────────────────────────────

# Query 14: Filtrar por Tipo = CONTA
query TicketsConta {
  ticketsPorTipo(tipo: CONTA) {
    id
    protocolo
    nomeCliente
  }
}

# ─────────────────────────────────────────────────────

# Query 15: Filtrar por Tipo = EMPRESTIMO
query TicketsEmprestimo {
  ticketsPorTipo(tipo: EMPRESTIMO) {
    id
    protocolo
    nomeCliente
  }
}

# ─────────────────────────────────────────────────────

# Query 16: Buscar Tickets CRÍTICOS e ABERTOS
# ⚠️ Combina dois filtros - tickets que precisam de ação imediata
query TicketsCriticosAbertos {
  ticketsCriticosAbertos {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
    descricao
  }
}

# ─────────────────────────────────────────────────────

# Query 17: Contar total de tickets
query TotalTickets {
  totalTickets
}

# ─────────────────────────────────────────────────────

# Query 18: Contar tickets com status ABERTO
query TotalTicketsAbertos {
  totalTicketsPorStatus(status: ABERTO)
}

# ─────────────────────────────────────────────────────

# Query 19: Contar tickets com status EMANALISE
query TotalTicketsEmAnalise {
  totalTicketsPorStatus(status: EMANALISE)
}

# ─────────────────────────────────────────────────────

# Query 20: Contar tickets com status RESOLVIDO
query TotalTicketsResolvidos {
  totalTicketsPorStatus(status: RESOLVIDO)
}

# ─────────────────────────────────────────────────────

# Query 21: Query Complexa - Buscar tudo de uma vez
# Demonstra como combinar múltiplas queries
query BuscarEstatisticas {
  todos: tickets {
    id
    protocolo
  }
  abertos: ticketsPorStatus(status: ABERTO) {
    id
    protocolo
  }
  criticos: ticketsPorSeveridade(severidade: CRITICA) {
    id
    protocolo
  }
  totalTickets
}

# =====================================================
# ✏️ MUTATIONS - MODIFICAÇÃO DE DADOS
# =====================================================

# Mutation 1: Criar um Novo Ticket
# ⚠️ SE severidade = CRITICA, dispara a subscription!
mutation CriarTicketCritico {
  criarTicket(input: {
    protocolo: "PIX-2024-999"
    nomeCliente: "João Silva"
    tipo: PIX
    severidade: CRITICA
    descricao: "Transferência PIX de R$ 5.000 não recebida após 2 horas de envio"
  }) {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
  }
}

# ─────────────────────────────────────────────────────

# Mutation 2: Criar um Novo Ticket de Fraude
mutation CriarTicketFraude {
  criarTicket(input: {
    protocolo: "FRAUDE-2024-888"
    nomeCliente: "Maria Santos"
    tipo: FRAUDE
    severidade: CRITICA
    descricao: "Movimento não autorizado na conta bancária"
  }) {
    id
    protocolo
    nomeCliente
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 3: Criar um Novo Ticket de Cartão
mutation CriarTicketCartao {
  criarTicket(input: {
    protocolo: "CARTAO-2024-777"
    nomeCliente: "Pedro Costa"
    tipo: CARTAO
    severidade: ALTA
    descricao: "Cartão bloqueado sem motivo aparente"
  }) {
    id
    protocolo
    nomeCliente
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 4: Criar um Novo Ticket de Conta
mutation CriarTicketConta {
  criarTicket(input: {
    protocolo: "CONTA-2024-666"
    nomeCliente: "Ana Oliveira"
    tipo: CONTA
    severidade: MEDIA
    descricao: "Dificuldade em fazer login na aplicação móvel"
  }) {
    id
    protocolo
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 5: Criar um Novo Ticket de Empréstimo
mutation CriarTicketEmprestimo {
  criarTicket(input: {
    protocolo: "EMPRESTIMO-2024-555"
    nomeCliente: "Carlos Mendes"
    tipo: EMPRESTIMO
    severidade: BAIXA
    descricao: "Dúvida sobre taxa de juros do empréstimo"
  }) {
    id
    protocolo
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 6: Atualizar Status para EM ANÁLISE
# Use o ID de um ticket que você quer atualizar
mutation AtualizarParaEmAnalise {
  atualizarStatusTicket(id: 1, novoStatus: EMANALISE) {
    id
    protocolo
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 7: Atualizar Status para RESOLVIDO
mutation AtualizarParaResolvido {
  atualizarStatusTicket(id: 1, novoStatus: RESOLVIDO) {
    id
    protocolo
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 8: Atualizar Severidade para ALTA
mutation AtualizarSeveridadeParaAlta {
  atualizarSeveridadeTicket(id: 1, novaSeveridade: ALTA) {
    id
    protocolo
    severidade
  }
}

# ─────────────────────────────────────────────────────

# Mutation 9: Atualizar Severidade para CRITICA
# ⚠️ Isso vai disparar o evento ticketCriticoCriado!
mutation EscalarParaCritico {
  atualizarSeveridadeTicket(id: 1, novaSeveridade: CRITICA) {
    id
    protocolo
    severidade
    status
  }
}

# ─────────────────────────────────────────────────────

# Mutation 10: Deletar um Ticket
mutation DeletarTicket {
  deletarTicket(id: 1)
}

# =====================================================
# 🔔 SUBSCRIPTIONS - EVENTOS EM TEMPO REAL
# =====================================================

# Subscription 1: Notificações de Tickets CRÍTICOS
# ⚠️ Recebe uma notificação cada vez que um ticket CRÍTICO é criado
# 
# Como usar:
# 1. Cole esta subscription no Banana Cake Pop
# 2. Clique em "Start"
# 3. Abra outra aba do Banana Cake Pop
# 4. Execute a mutation CriarTicketCritico
# 5. Volte para esta aba e você verá a notificação em tempo real!
subscription ReceberTicketsCriticos {
  ticketCriticoCriado {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    status
    dataCriacao
    descricao
  }
}

# ─────────────────────────────────────────────────────

# Subscription 2: Notificações de QUALQUER Ticket Criado
# Recebe uma notificação para cada novo ticket (não apenas críticos)
subscription ReceberTodosTickets {
  ticketCriado {
    id
    protocolo
    nomeCliente
    tipo
    severidade
    dataCriacao
  }
}

# =====================================================
# 💡 DICAS PARA TESTES
# =====================================================

# 1. TESTANDO SUBSCRIPTIONS:
#    - Abra 2 abas do Banana Cake Pop
#    - Aba 1: Cole a subscription e clique "Start"
#    - Aba 2: Execute uma mutation que cria ticket crítico
#    - Aba 1: Verá a notificação em tempo real!

# 2. TESTANDO COMBINADAS:
#    - Execute a query para contar tickets
#    - Execute uma mutation para criar novo ticket
#    - Execute a query novamente - o contador aumentou!

# 3. FILTRANDO DADOS:
#    - Use as queries para entender os dados
#    - Combine múltiplas queries em uma só

# 4. MONITORAMENTO:
#    - Keep a subscription ativa em background
#    - Receba alertas em tempo real de tickets críticos
#    - Integre com webhooks ou notificações push

# =====================================================
