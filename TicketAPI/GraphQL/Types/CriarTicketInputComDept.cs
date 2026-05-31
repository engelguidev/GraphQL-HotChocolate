using HotChocolate.Execution.Configuration;
using TicketAPI.Models;

namespace TicketAPI.GraphQL.Types;

/// <summary>
/// Input type para criar um novo ticket com departamento
/// Usado nas mutations de criação de ticket
/// </summary>
public class CriarTicketInputComDeptInput
{
    /// <summary>
    /// Protocolo único para o ticket (opcional - gerado automaticamente se não fornecido)
    /// </summary>
    public string? Protocolo { get; set; }

    /// <summary>
    /// Nome do cliente
    /// </summary>
    public required string NomeCliente { get; set; }

    /// <summary>
    /// Tipo de reclamação
    /// </summary>
    public TipoReclamacao Tipo { get; set; }

    /// <summary>
    /// Nível de severidade
    /// </summary>
    public NivelSeveridade Severidade { get; set; }

    /// <summary>
    /// Descrição da reclamação
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// ID do departamento responsável por este ticket
    /// </summary>
    public int DepartamentoId { get; set; }
}
