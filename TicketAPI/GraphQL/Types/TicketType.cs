using TicketAPI.Models;

namespace TicketAPI.GraphQL.Types;

/// <summary>
/// Tipo GraphQL para a entidade Ticket
/// Define a estrutura de dados que será retornada nas queries, mutations e subscriptions
/// </summary>
public class TicketType
{
    /// <summary>
    /// Identificador único do ticket
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número do protocolo único para referência
    /// </summary>
    public string Protocolo { get; set; } = string.Empty;

    /// <summary>
    /// Nome do cliente que realizou a reclamação
    /// </summary>
    public string NomeCliente { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de reclamação (PIX, Cartão, Conta, Fraude, Empréstimo)
    /// </summary>
    public TipoReclamacao Tipo { get; set; }

    /// <summary>
    /// Nível de severidade da reclamação
    /// </summary>
    public NivelSeveridade Severidade { get; set; }

    /// <summary>
    /// Status atual do ticket
    /// </summary>
    public StatusTicket Status { get; set; }

    /// <summary>
    /// Data e hora de criação do ticket
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Descrição adicional da reclamação
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// ID do departamento responsável por este ticket
    /// </summary>
    public int? DepartamentoId { get; set; }

    /// <summary>
    /// Departamento responsável por este ticket
    /// </summary>
    public DepartamentoType? Departamento { get; set; }

    /// <summary>
    /// Método para converter um modelo Ticket para TicketType
    /// </summary>
    /// <param name="ticket">Instância do modelo Ticket</param>
    public static TicketType FromTicket(Ticket ticket) =>
        new()
        {
            Id = ticket.Id,
            Protocolo = ticket.Protocolo,
            NomeCliente = ticket.NomeCliente,
            Tipo = ticket.Tipo,
            Severidade = ticket.Severidade,
            Status = ticket.Status,
            DataCriacao = ticket.DataCriacao,
            Descricao = ticket.Descricao,
            DepartamentoId = ticket.DepartamentoId,
            Departamento = ticket.Departamento != null ? DepartamentoType.FromDepartamento(ticket.Departamento) : null
        };
}
