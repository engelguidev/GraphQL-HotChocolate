using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.GraphQL.Types;
using TicketAPI.Models;

namespace TicketAPI.GraphQL.Mutations;

/// <summary>
/// Classe que define todas as mutations GraphQL disponíveis
/// As mutations permitem criar, atualizar ou deletar dados
/// Após cada mutation crítica, eventos são publicados para as subscriptions
/// </summary>
public class Mutation
{
    /// <summary>
    /// Mutation para criar um novo ticket de reclamação
    /// 
    /// Fluxo:
    /// 1. Recebe os dados de entrada (input)
    /// 2. Cria uma nova entidade Ticket
    /// 3. Salva no banco de dados
    /// 4. Se a severidade for CRITICA, publica um evento para a subscription
    /// 5. Retorna o ticket criado
    /// 
    /// Exemplo de uso GraphQL:
    /// mutation {
    ///   criarTicket(input: {
    ///     protocolo: "PIX-2024-999"
    ///     nomeCliente: "João Silva"
    ///     tipo: PIX
    ///     severidade: CRITICA
    ///     descricao: "Transferência não recebida"
    ///   }) {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///     status
    ///     dataCriacao
    ///   }
    /// }
    /// </summary>
    /// <param name="input">Dados de entrada para criar o ticket</param>
    /// <param name="context">DbContext injetado para salvar no banco</param>
    /// <param name="eventSender">ITopicEventSender para publicar eventos de subscriptions</param>
    /// <returns>O ticket recém-criado</returns>
    public async Task<TicketType> CriarTicket(
        CriarTicketInputComDeptInput input,
        TicketDbContext context,
        [Service] ITopicEventSender eventSender)
    {
        // Gera um protocolo único se não foi fornecido (verifica null OU vazio)
        string protocolo = string.IsNullOrWhiteSpace(input.Protocolo) 
            ? await GerarProtocoloUnico(context) 
            : input.Protocolo;

        // Cria nova instância de Ticket com status ABERTO e data atual
        var ticket = new Ticket
        {
            Protocolo = protocolo,
            NomeCliente = input.NomeCliente,
            Tipo = input.Tipo,
            Severidade = input.Severidade,
            Status = StatusTicket.ABERTO, // Status inicial sempre ABERTO
            DataCriacao = DateTime.UtcNow, // Data atual em UTC
            Descricao = input.Descricao,
            DepartamentoId = input.DepartamentoId // Adiciona o departamento
        };

        // Adiciona o ticket ao contexto do Entity Framework
        context.Tickets.Add(ticket);

        // Salva as alterações no banco de dados
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Tickets_Protocolo") ?? false)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro: Protocolo '{protocolo}' já existe no sistema. Use um protocolo diferente ou deixe em branco para geração automática.")
                    .SetCode("PROTOCOLO_DUPLICADO")
                    .Build());
        }
        catch (DbUpdateException)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("Erro ao salvar o ticket no banco de dados. Verifique os dados fornecidos.")
                    .SetCode("DB_ERROR")
                    .Build());
        }

        // Recarrega o ticket do banco para garantir que navegação está carregada
        var ticketRecarregado = await context.Tickets
            .Include(t => t.Departamento)
            .FirstOrDefaultAsync(t => t.Id == ticket.Id);

        if (ticketRecarregado == null)
            throw new InvalidOperationException("Erro ao recarregar o ticket criado.");

        ticket = ticketRecarregado;

        // Publica evento GERAL para todos os tickets criados
        // Todos os clientes conectados à subscription "ticketCriado" receberão a notificação
        await eventSender.SendAsync(
            "TicketCriado", // Nome do tópico para todos os tickets
            ticket); // Dados do evento

        // Se a severidade for CRITICA, publica também um evento específico para críticos
        if (input.Severidade == NivelSeveridade.CRITICA)
        {
            // Publica o evento na subscription "ticketCriticoCriado"
            // Todos os clientes conectados à subscription receberão a notificação
            await eventSender.SendAsync(
                "TicketCriticoCriado", // Nome do tópico para tickets críticos
                ticket); // Dados do evento
        }

        // Retorna o ticket criado em formato GraphQL
        return TicketType.FromTicket(ticket);
    }

    /// <summary>
    /// Gera um protocolo único com formato: PROT-{ANO}-{SEQUENCIA}
    /// Verifica os protocolos existentes no banco e incrementa o número sequencial
    /// </summary>
    private async Task<string> GerarProtocoloUnico(TicketDbContext context)
    {
        var anoAtual = DateTime.UtcNow.Year;
        var prefixo = $"PROT-{anoAtual}-";

        // Obtém todos os protocolos do ano atual
        var protocolosAnoAtual = await context.Tickets
            .Where(t => t.Protocolo.StartsWith(prefixo))
            .Select(t => t.Protocolo)
            .ToListAsync();

        // Extrai o número sequencial máximo
        int proximoNumero = 1;
        if (protocolosAnoAtual.Any())
        {
            var numeros = protocolosAnoAtual
                .Select(p => int.TryParse(p.Replace(prefixo, ""), out var num) ? num : 0)
                .Where(n => n > 0)
                .ToList();

            if (numeros.Any())
            {
                proximoNumero = numeros.Max() + 1;
            }
        }

        return $"{prefixo}{proximoNumero:D3}";
    }

    /// <summary>
    /// Mutation para atualizar o status de um ticket existente
    /// 
    /// Exemplo de uso GraphQL:
    /// mutation {
    ///   atualizarStatusTicket(id: 1, novoStatus: EMANALISE) {
    ///     id
    ///     protocolo
    ///     status
    ///   }
    /// }
    /// </summary>
    /// <param name="id">ID do ticket a atualizar</param>
    /// <param name="novoStatus">Novo status do ticket</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>O ticket atualizado ou erro se não encontrado</returns>
    public async Task<TicketType> AtualizarStatusTicket(
        int id,
        StatusTicket novoStatus,
        TicketDbContext context)
    {
        // Busca o ticket no banco
        var ticket = await context.Tickets.FindAsync(id);

        if (ticket is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro: Ticket com ID '{id}' não encontrado no sistema.")
                    .SetCode("TICKET_NAO_ENCONTRADO")
                    .Build());
        }

        // Atualiza o status
        ticket.Status = novoStatus;

        // Salva as alterações
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro ao atualizar o status do ticket {id}. Verifique os dados fornecidos.")
                    .SetCode("DB_ERROR")
                    .Build());
        }

        // Recarrega o ticket com o departamento
        var ticketAtualizado = await context.Tickets
            .Include(t => t.Departamento)
            .FirstOrDefaultAsync(t => t.Id == id);

        return TicketType.FromTicket(ticketAtualizado ?? ticket);
    }

    /// <summary>
    /// Mutation para atualizar a severidade de um ticket
    /// 
    /// Exemplo de uso GraphQL:
    /// mutation {
    ///   atualizarSeveridadeTicket(id: 1, novaSeveridade: ALTA) {
    ///     id
    ///     protocolo
    ///     severidade
    ///   }
    /// }
    /// </summary>
    /// <param name="id">ID do ticket a atualizar</param>
    /// <param name="novaSeveridade">Novo nível de severidade</param>
    /// <param name="context">DbContext injetado</param>
    /// <param name="eventSender">ITopicEventSender para publicar eventos</param>
    /// <returns>O ticket atualizado ou erro se não encontrado</returns>
    public async Task<TicketType> AtualizarSeveridadeTicket(
        int id,
        NivelSeveridade novaSeveridade,
        TicketDbContext context,
        [Service] ITopicEventSender eventSender)
    {
        // Busca o ticket no banco
        var ticket = await context.Tickets.FindAsync(id);

        if (ticket is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro: Ticket com ID '{id}' não encontrado no sistema.")
                    .SetCode("TICKET_NAO_ENCONTRADO")
                    .Build());
        }

        // Se a nova severidade for CRITICA, publica um evento
        if (novaSeveridade == NivelSeveridade.CRITICA && ticket.Severidade != NivelSeveridade.CRITICA)
        {
            // Ticket foi escalado para crítico - notifica as subscriptions
            await eventSender.SendAsync("TicketCriticoCriado", ticket);
        }

        // Atualiza a severidade
        ticket.Severidade = novaSeveridade;

        // Salva as alterações
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro ao atualizar a severidade do ticket {id}. Verifique os dados fornecidos.")
                    .SetCode("DB_ERROR")
                    .Build());
        }

        // Recarrega o ticket com o departamento
        var ticketAtualizado = await context.Tickets
            .Include(t => t.Departamento)
            .FirstOrDefaultAsync(t => t.Id == id);

        return TicketType.FromTicket(ticketAtualizado ?? ticket);
    }

    /// <summary>
    /// Mutation para deletar um ticket (soft delete conceitual - não remove do BD)
    /// Alternativamente, pode ser implementado como hard delete
    /// 
    /// Exemplo de uso GraphQL:
    /// mutation {
    ///   deletarTicket(id: 1)
    /// }
    /// </summary>
    /// <param name="id">ID do ticket a deletar</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>true se deletado com sucesso, ou erro se não encontrado</returns>
    public async Task<bool> DeletarTicket(int id, TicketDbContext context)
    {
        // Busca o ticket no banco
        var ticket = await context.Tickets.FindAsync(id);

        if (ticket is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro: Ticket com ID '{id}' não encontrado no sistema. Não é possível deletar um ticket que não existe.")
                    .SetCode("TICKET_NAO_ENCONTRADO")
                    .Build());
        }

        // Remove o ticket
        context.Tickets.Remove(ticket);

        // Salva as alterações
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Erro ao deletar o ticket {id}. Verifique se existem dependências ou tente novamente.")
                    .SetCode("DB_ERROR")
                    .Build());
        }

        return true;
    }
}
