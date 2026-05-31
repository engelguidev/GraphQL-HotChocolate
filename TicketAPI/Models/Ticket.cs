namespace TicketAPI.Models;

/// <summary>
/// Entidade que representa um ticket de reclamação crítica de cliente bancário
/// </summary>
public class Ticket
{
    /// <summary>
    /// Identificador único do ticket no banco de dados
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número do protocolo único para identificação do ticket
    /// Exemplo: "123456" ou "PRE-2024-001"
    /// </summary>
    public required string Protocolo { get; set; }

    /// <summary>
    /// Nome do cliente que realizou a reclamação
    /// </summary>
    public required string NomeCliente { get; set; }

    /// <summary>
    /// Tipo de reclamação (PIX, Cartão, Conta, Fraude, Empréstimo)
    /// </summary>
    public TipoReclamacao Tipo { get; set; }

    /// <summary>
    /// Nível de severidade da reclamação (Baixa, Média, Alta, Crítica)
    /// </summary>
    public NivelSeveridade Severidade { get; set; }

    /// <summary>
    /// Status atual do ticket (Aberto, Em Análise, Resolvido)
    /// Inicializado como "ABERTO" quando criado
    /// </summary>
    public StatusTicket Status { get; set; } = StatusTicket.ABERTO;

    /// <summary>
    /// Data e hora de criação do ticket
    /// Preenchida automaticamente no momento da criação
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Descrição adicional da reclamação
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// ID do departamento responsável por este ticket
    /// Chave estrangeira para Departamento
    /// </summary>
    public int? DepartamentoId { get; set; }

    /// <summary>
    /// Navegação para o departamento responsável
    /// </summary>
    public Departamento? Departamento { get; set; }
}
