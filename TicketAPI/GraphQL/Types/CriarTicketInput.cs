using TicketAPI.Models;

namespace TicketAPI.GraphQL.Types;

/// <summary>
/// Tipo de entrada para criação de um novo ticket
/// Define os campos necessários que o cliente deve fornecer
/// </summary>
public class CriarTicketInput
{
    /// <summary>
    /// Número do protocolo único do ticket
    /// Exemplo: "PIX-2024-001" ou "123456"
    /// </summary>
    public required string Protocolo { get; set; }

    /// <summary>
    /// Nome do cliente que está realizando a reclamação
    /// </summary>
    public required string NomeCliente { get; set; }

    /// <summary>
    /// Tipo de reclamação (PIX, Cartão, Conta, Fraude, Empréstimo)
    /// </summary>
    public TipoReclamacao Tipo { get; set; }

    /// <summary>
    /// Nível de severidade da reclamação
    /// Sendo crítico, dispara uma subscription para notificar
    /// </summary>
    public NivelSeveridade Severidade { get; set; }

    /// <summary>
    /// Descrição opcional da reclamação
    /// </summary>
    public string? Descricao { get; set; }
}
