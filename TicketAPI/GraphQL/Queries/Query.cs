using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.GraphQL.Types;
using TicketAPI.Models;

namespace TicketAPI.GraphQL.Queries;

/// <summary>
/// Classe que define todas as queries GraphQL disponíveis
/// As queries permitem leitura de dados sem modificações
/// </summary>
public class Query
{
    /// <summary>
    /// Query para buscar todos os tickets do banco de dados
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   tickets {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///     tipo
    ///     severidade
    ///     status
    ///   }
    /// }
    /// </summary>
    /// <param name="context">DbContext injetado para acessar o banco de dados</param>
    /// <returns>Lista de todos os tickets</returns>
    public async Task<List<TicketType>> GetTickets(TicketDbContext context)
    {
        var tickets = await context.Tickets
            .Include(t => t.Departamento)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();

        return tickets
            .Select(TicketType.FromTicket)
            .ToList();
    }

    /// <summary>
    /// Query para buscar um ticket específico pelo seu ID
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketPorId(id: 1) {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///     descricao
    ///   }
    /// }
    /// </summary>
    /// <param name="id">ID do ticket a buscar</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>O ticket encontrado ou null se não existir</returns>
    public async Task<TicketType?> GetTicketPorId(int id, TicketDbContext context)
    {
        var ticket = await context.Tickets
            .Include(t => t.Departamento)
            .FirstOrDefaultAsync(t => t.Id == id);

        return ticket is null ? null : TicketType.FromTicket(ticket);
    }

    /// <summary>
    /// Query para buscar um ticket específico pelo seu número de protocolo
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketPorProtocolo(protocolo: "PIX-2024-001") {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///   }
    /// }
    /// </summary>
    /// <param name="protocolo">Número do protocolo do ticket</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>O ticket encontrado ou null se não existir</returns>
    public async Task<TicketType?> GetTicketPorProtocolo(string protocolo, TicketDbContext context)
    {
        var ticket = await context.Tickets
            .Include(t => t.Departamento)
            .FirstOrDefaultAsync(t => t.Protocolo == protocolo);

        return ticket is null ? null : TicketType.FromTicket(ticket);
    }

    /// <summary>
    /// Query para filtrar tickets por status
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketsPorStatus(status: CRITICA) {
    ///     id
    ///     protocolo
    ///     severidade
    ///   }
    /// }
    /// </summary>
    /// <param name="status">Status para filtro</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Lista de tickets com o status especificado</returns>
    public async Task<List<TicketType>> GetTicketsPorStatus(StatusTicket status, TicketDbContext context)
    {
        var tickets = await context.Tickets
            .Include(t => t.Departamento)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();

        return tickets
            .Select(TicketType.FromTicket)
            .ToList();
    }

    /// <summary>
    /// Query para filtrar tickets por severidade
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketsPorSeveridade(severidade: CRITICA) {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///     tipo
    ///   }
    /// }
    /// </summary>
    /// <param name="severidade">Nível de severidade para filtro</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Lista de tickets com o nível de severidade especificado</returns>
    public async Task<List<TicketType>> GetTicketsPorSeveridade(NivelSeveridade severidade, TicketDbContext context)
    {
        var tickets = await context.Tickets
            .Include(t => t.Departamento)
            .Where(t => t.Severidade == severidade)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();

        return tickets
            .Select(TicketType.FromTicket)
            .ToList();
    }

    /// <summary>
    /// Query para filtrar tickets por tipo de reclamação
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketsPorTipo(tipo: PIX) {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///   }
    /// }
    /// </summary>
    /// <param name="tipo">Tipo de reclamação para filtro</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Lista de tickets do tipo especificado</returns>
    public async Task<List<TicketType>> GetTicketsPorTipo(TipoReclamacao tipo, TicketDbContext context)
    {
        var tickets = await context.Tickets
            .Include(t => t.Departamento)
            .Where(t => t.Tipo == tipo)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();

        return tickets
            .Select(TicketType.FromTicket)
            .ToList();
    }

    /// <summary>
    /// Query para buscar tickets críticos ainda abertos
    /// Esta é uma query de conveniência que combina dois filtros comuns
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   ticketsCriticosAbertos {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///   }
    /// }
    /// </summary>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Lista de tickets críticos e abertos</returns>
    public async Task<List<TicketType>> GetTicketsCriticosAbertos(TicketDbContext context)
    {
        var tickets = await context.Tickets
            .Include(t => t.Departamento)
            .Where(t => t.Severidade == NivelSeveridade.CRITICA && t.Status == StatusTicket.ABERTO)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();

        return tickets
            .Select(TicketType.FromTicket)
            .ToList();
    }

    /// <summary>
    /// Query para contar o número total de tickets
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   totalTickets
    /// }
    /// </summary>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Quantidade total de tickets</returns>
    public async Task<int> GetTotalTickets(TicketDbContext context)
    {
        return await context.Tickets.CountAsync();
    }

    /// <summary>
    /// Query para contar tickets por status
    /// 
    /// Exemplo de uso GraphQL:
    /// query {
    ///   totalTicketsPorStatus(status: CRITICA)
    /// }
    /// </summary>
    /// <param name="status">Status para contagem</param>
    /// <param name="context">DbContext injetado</param>
    /// <returns>Quantidade de tickets com o status especificado</returns>
    public async Task<int> GetTotalTicketsPorStatus(StatusTicket status, TicketDbContext context)
    {
        return await context.Tickets
            .Where(t => t.Status == status)
            .CountAsync();
    }
}
